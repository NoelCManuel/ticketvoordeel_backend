using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Org.BouncyCastle.Asn1.Mozilla;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ticketvoordeel.Helpers
{
    public class S3UploadHelper
    {
        private static String accessKey = "AKIAIVX65YHHW32PTO5Q";
        private static String accessSecret = "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";
        private static String bucket = "ticketvoordeel";
        public string GenerateFileName(string extension)
        {
            return $@"{DateTime.Now.Ticks}." + extension;
        }

        public async Task<string> UploadObject(IFormFile file, string folder, IWebHostEnvironment _hostingEnvironment = null)
        {
            //var client = new AmazonS3Client(accessKey, accessSecret, Amazon.RegionEndpoint.EUCentral1);

            


            var uploads = Path.Combine(_hostingEnvironment.WebRootPath, @"uploads\blog\");

            if (!System.IO.Directory.Exists(uploads))
            {
                System.IO.Directory.CreateDirectory(uploads);
            }



            byte[] fileBytes = new Byte[file.Length];
            file.OpenReadStream().Read(fileBytes, 0, Int32.Parse(file.Length.ToString()));
            var fileName = Guid.NewGuid() + file.FileName.Trim().Replace(" ", "_");
            var value = Path.Combine(uploads + fileName);
            if (file != null)
            {                
                if (file.Length > 0)
                {
                    using (var fileStream = new FileStream(value, FileMode.Create))
                    {

                        file.CopyTo(fileStream);
                    }
                }
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
            }

            //PutObjectResponse response = null;

            //using (var stream = new MemoryStream(fileBytes))
            //{
            //    var request = new PutObjectRequest
            //    {
            //        BucketName = bucket,
            //        Key = folder + "/" + fileName,
            //        InputStream = stream,
            //        ContentType = file.ContentType,
            //        CannedACL = S3CannedACL.PublicRead
            //    };

            //    response = await client.PutObjectAsync(request);
            //};

            //if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            //{
            //    return fileName;
            //}
            //else
            //{
            //    return "error";
            //}
            return fileName;
        }

        //public async Task<bool> RemoveObject(String fileName)
        //{
        //    try
        //    {
        //        var client = new AmazonS3Client(accessKey, accessSecret, Amazon.RegionEndpoint.EUCentral1);

        //        var request = new DeleteObjectRequest
        //        {
        //            BucketName = bucket,
        //            Key = fileName
        //        };

        //        var response = await client.DeleteObjectAsync(request);

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }                       
        //}
    }
}
