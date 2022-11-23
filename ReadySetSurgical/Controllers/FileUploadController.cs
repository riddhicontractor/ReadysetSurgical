using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Amazon.Textract;
using Amazon.Textract.Model;
using AWS_S3_Demo.Data;
using AWS_S3_Demo.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace ReadySetSurgical.Controllers
{
    public class FileUploadController : Controller
    {
        private readonly IAmazonS3 s3Client;
        private string BucketName = "aws-s3-bucket-demo-123";
        private IWebHostEnvironment _webHostEnvironment;
        public string ?InvoiceNumber;
        public string? VendorName;
        public string? ReceiverName;
        public string? OtherName;
        readonly DataContext _dataContext;
        public int ProcessedFiles = 0;
        public int UnprocessedFiles = 0;

        public FileUploadController(IWebHostEnvironment webHostEnvironment, IAmazonS3 s3Client, DataContext dataContext)
        {
            this.s3Client = s3Client;
            _webHostEnvironment = webHostEnvironment;
            _dataContext = dataContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> FileUploadAsync(List<IFormFile> formFile)
        {
            try
            {
                var bucketExists = await AmazonS3Util.DoesS3BucketExistV2Async(s3Client, BucketName);
                if (!bucketExists) //create a new bucket in AWS (if not exists)
                {
                    var bucketRequest = new PutBucketRequest()
                    {
                        BucketName = BucketName,
                        UseClientRegion = true
                    };
                    await s3Client.PutBucketAsync(bucketRequest);
                }

                using (var textractClient = new AmazonTextractClient(RegionEndpoint.APSouth1))
                {
                    using (var s3Client = new AmazonS3Client(RegionEndpoint.APSouth1))
                    {
                        if (formFile.Count > 0)
                        {
                            foreach (var file in formFile)
                            {
                                var objectRequest = new PutObjectRequest()
                                {
                                    BucketName = BucketName,
                                    Key = Path.GetFileName(file.FileName),
                                    InputStream = file.OpenReadStream()
                                };

                                //add file to bucket
                                var response = await s3Client.PutObjectAsync(objectRequest);

                                if (response != null)
                                {
                                 // AWS Textract - Expense API

                                    var expenseRequest = await textractClient.StartExpenseAnalysisAsync(new StartExpenseAnalysisRequest
                                    {
                                        DocumentLocation = new DocumentLocation
                                        {
                                            S3Object = new Amazon.Textract.Model.S3Object
                                            {
                                                Bucket = BucketName,
                                                Name = objectRequest.Key
                                            }
                                        }
                                    });

                                    var expenseRequestId = expenseRequest.ResponseMetadata.RequestId;

                                    var GetExpenseAnalysis = new GetExpenseAnalysisRequest
                                    {
                                        JobId = expenseRequest.JobId
                                    };

                                    GetExpenseAnalysisResponse getExpenseAnalysisResponse;

                                    do
                                    {
                                        Thread.Sleep(1000);
                                        getExpenseAnalysisResponse = await textractClient.GetExpenseAnalysisAsync(GetExpenseAnalysis);
                                    } while (getExpenseAnalysisResponse.JobStatus == JobStatus.IN_PROGRESS);

                                    // If the job was successful loop through the pages of results and print the detected text

                                    if (getExpenseAnalysisResponse.JobStatus == JobStatus.SUCCEEDED)
                                    {
                                        do
                                        {
                                            List<ExpenseDocument> docs = getExpenseAnalysisResponse.ExpenseDocuments;

                                            List<ExpenseField> _summaryFields = new List<ExpenseField>();

                                            List<ExpenseGroupProperty> _expenseGroupProperties = new List<ExpenseGroupProperty>();

                                            foreach (var doc in docs)
                                            {
                                                _summaryFields = doc.SummaryFields;

                                                if (_summaryFields.Count > 0)
                                                {
                                                    foreach (var field in _summaryFields)
                                                    {
                                                        if (field.Type.Text == "INVOICE_RECEIPT_ID")
                                                        {
                                                            Console.WriteLine($"Invoice Id = {field.ValueDetection.Text}");
                                                            InvoiceNumber = field.ValueDetection.Text;
                                                        }
                                                        _expenseGroupProperties = field.GroupProperties;

                                                        foreach (var group in _expenseGroupProperties)
                                                        {
                                                            if (group.Types.Contains("VENDOR"))
                                                            {
                                                                if (field.Type.Text == "NAME")
                                                                {
                                                                    Console.WriteLine($"Vendor Name = {field.ValueDetection.Text}");
                                                                    VendorName = field.ValueDetection.Text;
                                                                }
                                                            }
                                                            else if (group.Types.Contains("RECEIVER"))
                                                            {
                                                                if (field.Type.Text == "NAME")
                                                                {
                                                                    Console.WriteLine($"Receiver Name = {field.ValueDetection.Text}");
                                                                    ReceiverName = field.ValueDetection.Text;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (field.Type.Text == "NAME")
                                                                {
                                                                    Console.WriteLine($"Receiver Name = {field.ValueDetection.Text}");
                                                                    OtherName = field.ValueDetection.Text;
                                                                }
                                                            }
                                                        }
                                                    }

                                                //add to db start
                                                    InvoiceDetails invoiceDetail = new InvoiceDetails()
                                                    {
                                                        InvoiceNumber = InvoiceNumber,
                                                        VendorName = VendorName,
                                                        ReceiverName = ReceiverName != "" ? ReceiverName : OtherName,
                                                        CreatedAt = DateTime.Now,
                                                        FileName = file.FileName
                                                    };

                                                    _dataContext.Add(invoiceDetail);
                                                    _dataContext.SaveChanges();

                                                    ProcessedFiles++;
                                                //end

                                                    // Check to see if there are no more pages of data. If no then break.
                                                    if (string.IsNullOrEmpty(getExpenseAnalysisResponse.NextToken))
                                                    {
                                                        break;
                                                    }

                                                    GetExpenseAnalysis.NextToken = getExpenseAnalysisResponse.NextToken;
                                                    getExpenseAnalysisResponse = await textractClient.GetExpenseAnalysisAsync(GetExpenseAnalysis);
                                                }
                                                else
                                                {
                                                    UnprocessedFiles++;
                                                }
                                            }

                                        } while (!string.IsNullOrEmpty(getExpenseAnalysisResponse.NextToken));
                                    }
                                    else
                                    {
                                        UnprocessedFiles++;
                                    }
                                }
                                else
                                {
                                    ViewBag.FileFailedToUpload = "Uploading file to AWS S3 failed!";
                                }
                            }
                        }
                        else
                        {
                            ViewBag.FileFailedToUpload = "Failed!";
                        }
                    }
                }

                if (ProcessedFiles != 0)
                {
                    ViewBag.FileCreated = "Total Processed Files = " + ProcessedFiles;
                }
                if (UnprocessedFiles != 0)
                {
                    ViewBag.FileFailedToUpload = "Total Unprocessed Files = " + UnprocessedFiles;
                }
            }
            catch(Exception ex)
            {
                ViewBag.FileFailedToUpload = "Failed!" +ex;
            }
            return View("Index");
        }
    }
}