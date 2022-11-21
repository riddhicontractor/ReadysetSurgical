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
        private string BucketName = "aws-s3-bucket-demo-123";
        private IWebHostEnvironment _webHostEnvironment;
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
            if (!bucketExists)
            {
                var bucketRequest = new PutBucketRequest()
                {
                    BucketName = BucketName,
                    UseClientRegion = true
                };
                await s3Client.PutBucketAsync(bucketRequest); //create a new bucket in AWS
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
                            //Key = $"{DateTime.Now:yyyyMMddhhmmss}-{formFile.FileName}"
                            Key = Path.GetFileName(file.FileName),
                            InputStream = file.OpenReadStream()
                        };
                        var response = await s3Client.PutObjectAsync(objectRequest);
                        if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                        {                        
                            ViewBag.msg = "File uploaded to AWS S3 successfully !";
                        }
                        else
                        {
                            ViewBag.FileFailedToUpload = "Failed !";
                        }
                    }
                }
            }
            return View("Index");
        }
    }
}