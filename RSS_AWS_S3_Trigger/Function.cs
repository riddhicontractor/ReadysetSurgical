using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.S3;
using Amazon.Textract.Model;
using Amazon.Textract;
using Amazon;
using System.Data.SqlClient;
using ReadySetSurgical.Model;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AWS_S3_Lambda_Trigger;

public class Function
{
    public string invoiceNumber = string.Empty;
    public string vendorName = string.Empty;
    public string receiverName = string.Empty;
    public string otherName = string.Empty;
    public string connectionString = "server=sample-database.c53wji5mnp4g.ap-south-1.rds.amazonaws.com;User Id=Admin;Password=Jz7XXc8iqCHjJTL;database=sample;Trusted_Connection=True;TrustServerCertificate=Yes;Integrated Security=false;";

    IAmazonS3 S3Client { get; set; }

    /// <summary>
    /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
    /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
    /// region the Lambda function is executed in.
    /// </summary>
    public Function()
    {
        S3Client = new AmazonS3Client();
    }

    /// <summary>
    /// Constructs an instance with a preconfigured S3 client. This can be used for testing the outside of the Lambda environment.
    /// </summary>
    /// <param name="s3Client"></param>
    public Function(IAmazonS3 s3Client)
    {
        this.S3Client = s3Client;
    }

    /// <summary>
    /// This method is called for every Lambda invocation. This method takes in an S3 event object and can be used 
    /// to respond to S3 notifications.
    /// It invokes the AWS Textract Expense API by passing the AWS S3 file and will extract data from it.
    /// </summary>
    /// <param name="evnt"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task<string?> FunctionHandler(S3Event evnt, ILambdaContext context)
    {
        var s3Event = evnt.Records?[0].S3;
        if (s3Event == null)
        {
            return null;
        }

        try
        {
            var response = await this.S3Client.GetObjectMetadataAsync(s3Event.Bucket.Name, s3Event.Object.Key);
            var file = await this.S3Client.GetObjectAsync(s3Event.Bucket.Name, s3Event.Object.Key);
            using var reader = new StreamReader(file.ResponseStream);
            var fileContents = await reader.ReadToEndAsync();

            context.Logger.LogInformation($"Bucket Name: {file.BucketName}");
            context.Logger.LogInformation($"File Name: {file.Key}");

            using (var textractClient = new AmazonTextractClient(RegionEndpoint.APSouth1))
            {
                using AmazonS3Client s3Client = new(RegionEndpoint.APSouth1);
                context.Logger.LogInformation("Start document detection job");

            //AWS Textract Expense API

                var expenseRequest = await textractClient.StartExpenseAnalysisAsync(new StartExpenseAnalysisRequest
                {
                    DocumentLocation = new DocumentLocation
                    {
                        S3Object = new S3Object
                        {
                            Bucket = file.BucketName,
                            Name = file.Key
                        }
                    }
                });

                var expenseRequestId = expenseRequest.ResponseMetadata.RequestId;

                context.Logger.LogInformation("expenseRequestId = " + expenseRequestId);

                var GetExpenseAnalysis = new GetExpenseAnalysisRequest
                {
                    JobId = expenseRequest.JobId
                };

                context.Logger.LogInformation("JobId = " + GetExpenseAnalysis.JobId);

                context.Logger.LogInformation("Poll for detect job to complete");

                GetExpenseAnalysisResponse getExpenseAnalysisResponse;

                // Poll till job is no longer in progress.
                do
                {
                    Thread.Sleep(1000);
                    getExpenseAnalysisResponse = await textractClient.GetExpenseAnalysisAsync(GetExpenseAnalysis);

                } while (getExpenseAnalysisResponse.JobStatus == JobStatus.IN_PROGRESS);

                context.Logger.LogInformation("Print out results if the job was successful!");

                // If the job was successful loop through the pages of results and print the detected text
                if (getExpenseAnalysisResponse.JobStatus == JobStatus.SUCCEEDED)
                {
                    do
                    {
                        List<ExpenseDocument> docs = getExpenseAnalysisResponse.ExpenseDocuments;

                        foreach (var doc in docs)
                        {
                            List<ExpenseField> _summaryFields = doc.SummaryFields;

                            if (_summaryFields.Count > 0)
                            {
                                foreach (var field in _summaryFields)
                                {
                                    if (field.Type.Text == "INVOICE_RECEIPT_ID")
                                    {
                                        context.Logger.LogInformation($"Invoice Id = {field.ValueDetection.Text}");
                                        invoiceNumber = field.ValueDetection.Text;
                                    }
                                    List<ExpenseGroupProperty> _expenseGroupProperties = field.GroupProperties;

                                    foreach (var group in _expenseGroupProperties)
                                    {
                                        if (group.Types.Contains("VENDOR"))
                                        {
                                            if (field.Type.Text == "NAME")
                                            {
                                                context.Logger.LogInformation($"Vendor Name = {field.ValueDetection.Text}");
                                                vendorName = field.ValueDetection.Text;
                                            }
                                        }
                                        else if (group.Types.Contains("RECEIVER"))
                                        {
                                            if (field.Type.Text == "NAME")
                                            {
                                                context.Logger.LogInformation($"Receiver Name = {field.ValueDetection.Text}");
                                                receiverName = field.ValueDetection.Text;
                                            }
                                        }
                                        else
                                        {
                                            if (field.Type.Text == "NAME")
                                            {
                                                context.Logger.LogInformation($"Name = {field.ValueDetection.Text}");
                                                otherName = field.ValueDetection.Text;
                                            }
                                        }
                                    }
                                }

                                InvoiceDetail detail = new()
                                {
                                    InvoiceNumber = invoiceNumber,
                                    VendorName = vendorName,
                                    FileName = file.Key
                                };
                                if (receiverName != "")
                                {
                                    detail.ReceiverName = receiverName;
                                }
                                else if (receiverName != null)
                                {
                                    detail.ReceiverName = otherName;
                                }
                                else
                                {
                                    detail.ReceiverName = otherName;
                                }

                                bool isInvoiceDetailAdded = AddInvoiceDetails(detail, context);

                                if (isInvoiceDetailAdded)
                                {
                                    context.Logger.LogInformation($"{file.Key} - File has been extracted successfully!");
                                }

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
                                ErrorLog log = new()
                                {
                                    FileName = file.Key
                                };

                                bool isErrorLogAdded = AddErrorLogDetails(log, context);

                                if(isErrorLogAdded)
                                {
                                    context.Logger.LogInformation($"{file.Key} - File has no summary fileds to extract. Please upload a valid Invoice or Reciept!");
                                }
                            }
                        }
                    } while (!string.IsNullOrEmpty(getExpenseAnalysisResponse.NextToken));
                }
                else
                {
                    context.Logger.LogInformation($"Job failed with message: {getExpenseAnalysisResponse.StatusMessage}");
                }
            }
            return response.Headers.ContentType;
        }
        catch (Exception e)
        {
            context.Logger.LogInformation($"Error getting object {s3Event.Object.Key} from bucket {s3Event.Bucket.Name}. Make sure they exist and your bucket is in the same region as this function.");
            context.Logger.LogInformation(e.Message);
            context.Logger.LogInformation(e.StackTrace);
            throw;
        }
    }

    /// <summary>
    /// This method takes the successfull extracted files from Textract API and will stored the extracted data in AWS RDS.
    /// </summary>
    /// <param name="invoiceDetail"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    private bool AddInvoiceDetails(InvoiceDetail invoiceDetail, ILambdaContext context)
    {
        using var conn = new SqlConnection(connectionString);
        context.Logger.LogInformation("Try to connect to RDS...");

        conn.Open();

        context.Logger.LogInformation("Successfully connected to RDS!");

        SqlCommand cmd = new("INSERT INTO InvoiceDetail (InvoiceNumber, VendorName, ReceiverName, CreatedAt, FileName) VALUES ('" + invoiceDetail.InvoiceNumber + "', '" + invoiceDetail.VendorName + "', '" + invoiceDetail.ReceiverName + "', '" + DateTime.Now + "', '" + invoiceDetail.FileName + "');", conn);

        int success = cmd.ExecuteNonQuery();

        context.Logger.LogInformation("Extracted Invoice details inserted in database successfully!");

        conn.Close();

        if (success == 1)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// This method takes the invalid files and will stored the file details in AWS RDS.
    /// </summary>
    /// <param name="errorLog"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    private bool AddErrorLogDetails(ErrorLog errorLog, ILambdaContext context)
    {
        using var conn = new SqlConnection(connectionString);
        context.Logger.LogInformation("Try to connect to RDS...");

        conn.Open();

        context.Logger.LogInformation("Successfully connected to RDS.");

        SqlCommand cmd = new("INSERT INTO ErrorLog (CreatedAt, FileName) VALUES ('" + DateTime.Now + "', '" + errorLog.FileName + "');", conn);

        int success = cmd.ExecuteNonQuery();

        context.Logger.LogInformation("Error File details inserted in database successfully!");

        conn.Close();

        if (success == 1)
        {
            return true;
        }
        return false;
    }
}