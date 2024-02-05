using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Common.Settings.Amazon;
using Microsoft.AspNetCore.Http;
using Services.Helpers;
using Services.Interfaces;

namespace Services.Implementation
{
    public class AmazonS3BucketService: IAmazonS3BucketService
    {
        private readonly AmazonS3BucketSettings _settings;

        public AmazonS3BucketService(AmazonS3BucketSettings settings)
        {
            _settings = settings;
        }

        public async Task<string> UploadFile(string fileSource, string fileName, string fileData, string contentType, bool isNeedUniqueName = true)
        {
            if (isNeedUniqueName)
            {
                fileName = fileName.GetUniqueFileName();
            }
            fileName = fileName.Replace(' ', '_');
            var bucketSource = $"{_settings.BucketName}/{fileSource.Replace(' ', '_')}";
            var fileByteSource = Convert.FromBase64String(fileData);
            var fileMemoryStream = new MemoryStream(fileByteSource);
            var url = "";

            using (IAmazonS3 client = new AmazonS3Client(_settings.AccessKeyId, _settings.SecretAccessKey, RegionEndpoint.USEast1))
            {
                var request = new PutObjectRequest
                {
                    Key = fileName,
                    CannedACL = S3CannedACL.PublicRead,
                    BucketName = bucketSource,
                    ContentType = contentType,
                    InputStream = fileMemoryStream
                };
                var response = await client.PutObjectAsync(request);
                if (response.HttpStatusCode == HttpStatusCode.OK)
                {
                    url = !string.IsNullOrWhiteSpace(_settings.CDNUrl) ? $"{_settings.CDNUrl}/{fileSource.Replace(' ', '_')}/{request.Key}" :
                        $"https://s3.{RegionEndpoint.USEast1.SystemName}.amazonaws.com/{bucketSource}/{request.Key}";
                    url = AmazonS3Util.UrlEncode(url, true);
                }
            }
            return url;
        }

        public async Task<string> UploadSingleFile(string fileSource, IFormFile file, bool isNeedUniqueName = true)
        {
            var fileName = "";
            if (isNeedUniqueName)
            {
                fileName = file.FileName.GetUniqueFileName();
            }
            fileName = fileName.Replace(' ', '_');
            var bucketSource = $"{_settings.BucketName}/{fileSource.Replace(' ', '_')}";
            var fileMemoryStream = new MemoryStream();
            await file.CopyToAsync(fileMemoryStream);
            var url = "";

            using (IAmazonS3 client = new AmazonS3Client(_settings.AccessKeyId, _settings.SecretAccessKey, RegionEndpoint.USEast1))
            {
                var request = new PutObjectRequest
                {
                    Key = fileName,
                    CannedACL = S3CannedACL.PublicRead,
                    BucketName = bucketSource,
                    ContentType = file.ContentType,
                    InputStream = fileMemoryStream
                };
                var response = await client.PutObjectAsync(request);
                if (response.HttpStatusCode == HttpStatusCode.OK)
                {
                    url = !string.IsNullOrWhiteSpace(_settings.CDNUrl) ? $"{_settings.CDNUrl}/{fileSource.Replace(' ', '_')}/{request.Key}" :
                        $"https://s3.{RegionEndpoint.USEast1.SystemName}.amazonaws.com/{bucketSource}/{request.Key}";
                    url = AmazonS3Util.UrlEncode(url, true);
                }
            }
            return url;
        }

        public async Task<bool> DeleteBucket(string keyName)
        {
            var key = keyName.Replace($"https://s3.{RegionEndpoint.USEast1.SystemName}.amazonaws.com/", "");
            var isSuccess = false;
            using (IAmazonS3 client = new AmazonS3Client(_settings.AccessKeyId, _settings.SecretAccessKey, RegionEndpoint.USEast1))
            {
                var deleteObjectRequest = new DeleteObjectRequest
                {
                    BucketName = _settings.BucketName,
                    Key = key
                };
                var response = await client.DeleteObjectAsync(deleteObjectRequest);
                if(response.HttpStatusCode == HttpStatusCode.OK)
                {
                    isSuccess = true;
                }
            }
            return isSuccess;
        }
    }
}
