using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace ReadySetSurgical.Controllers
{
    public class FileUploadController : Controller
    {
        private readonly IAmazonS3 s3Client;
        private string BucketName = "aws-s3-demo-data"; //congifure Lamdbda Function
        private IWebHostEnvironment _webHostEnvironment;
        int UploadedFiles = 0;
        int ErrorFiles = 0;
        public FileUploadController(IWebHostEnvironment webHostEnvironment, IAmazonS3 s3Client)
        {
            this.s3Client = s3Client;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> FileUploadAsync(List<IFormFile> formFile)
        {
            var bucketExists = await AmazonS3Util.DoesS3BucketExistV2Async(s3Client, BucketName);
            if (!bucketExists) //create a new bucket in AWS S3(if not exists)
            {
                var bucketRequest = new PutBucketRequest()
                {
                    BucketName = BucketName,
                    UseClientRegion = true
                };
                await s3Client.PutBucketAsync(bucketRequest);
            }

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

                        //add files to S3
                        var response = await s3Client.PutObjectAsync(objectRequest);
                        if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                        {
                            UploadedFiles++;
                        }
                        else
                        {
                            ErrorFiles++;
                        }
                    }
                }
            }

            if(UploadedFiles > 0)
            {
                ViewBag.FileCreated = "Total Uploaded Files to AWS S3 = " + UploadedFiles;
            }
            if (ErrorFiles > 0)
            {
                ViewBag.FileFailedToUpload = "Error Files = " + ErrorFiles;
            }

            return View("Index");
        }
    }
}