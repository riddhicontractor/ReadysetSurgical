using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Microsoft.AspNetCore.Mvc;

namespace ReadySetSurgical.Controllers
{
    public class FileUploadController : Controller
    {
        private readonly IAmazonS3 _s3Client;
        public string bucketName = "aws-s3-demo-data";
        public IWebHostEnvironment _webHostEnvironment;
        public int uploadedFiles = 0;
        public int errorFiles = 0;

        /// <summary>
        /// Constructs an instance with a preconfigured S3 client and webHostEnvironment.
        /// </summary>
        /// <param name="webHostEnvironment"></param>
        /// <param name="s3Client"></param>
        public FileUploadController(IWebHostEnvironment webHostEnvironment, IAmazonS3 s3Client)
        {
            this._s3Client = s3Client;
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// This method is called for every FileUpload Controller invocation.
        /// </summary>
        /// <returns></returns>
        public IActionResult FileUpload()
        {
            return View();
        }

        /// <summary>
        /// This method is called for uploading files/folder to AWS S3. It create a new bucket in AWS S3(if not exists).
        /// It returns the total number of files uploaded to AWS S3.
        /// </summary>
        /// <param name="formFile"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> FileUploadAsync(List<IFormFile> formFile)
        {
            var bucketExists = await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName);

        //create a new bucket in AWS S3(if not exists)
            if (!bucketExists) 
            {
                var bucketRequest = new PutBucketRequest()
                {
                    BucketName = bucketName,
                    UseClientRegion = true
                };
                await _s3Client.PutBucketAsync(bucketRequest);
            }

            using (var s3Client = new AmazonS3Client(RegionEndpoint.APSouth1))
            {
                if (formFile.Count > 0)
                {
                    foreach (var file in formFile)
                    {                       
                        var objectRequest = new PutObjectRequest()
                        {
                            BucketName = bucketName,
                            Key = Path.GetFileName(file.FileName),
                            InputStream = file.OpenReadStream()
                        };

                    //add files to AWS S3
                        var response = await s3Client.PutObjectAsync(objectRequest);
                        if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                        {
                            uploadedFiles++;
                        }
                        else
                        {
                            errorFiles++;
                        }
                    }
                }
            }

            if(uploadedFiles > 0)
            {
                ViewBag.FileCreated = "Total Uploaded Files to AWS S3 = " + uploadedFiles;
            }
            if (errorFiles > 0)
            {
                ViewBag.FileFailedToUpload = "Error Files = " + errorFiles;
            }

            return View("FileUpload");
        }
    }
}