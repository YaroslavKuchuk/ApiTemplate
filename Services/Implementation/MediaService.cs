using System.Threading.Tasks;
using Common.Settings.Amazon;
using Microsoft.AspNetCore.Http;
using Services.Interfaces;
using Services.Model.Content;

namespace Services.Implementation
{
    public class MediaService: IMediaService
    {
        private readonly AmazonS3BucketService _amazonS3BucketService;

        public MediaService(ISettingService settingService)
        {
            var amazonBucketSettings = settingService.GetSettings<AmazonS3BucketSettings>().Result;
            _amazonS3BucketService = new AmazonS3BucketService(amazonBucketSettings);
        }

        public async Task<string> UploadFileToBucketFromBase64(Base64FileModel model, long userId, string folderName = null, bool isNeedUniqueName = true)
        {
            return await _amazonS3BucketService.UploadFile(string.IsNullOrWhiteSpace(folderName) ? userId.ToString() : $@"{userId}/{folderName}", model.FileName, model.Base64, model.ContentType, isNeedUniqueName);
        }

        public async Task<string> UploadFileToBucketFromFile(IFormFile file, long userId, string folderName = null, bool isNeedUniqueName = true)
        {
            return await _amazonS3BucketService.UploadSingleFile(string.IsNullOrWhiteSpace(folderName) ? userId.ToString() : $@"{userId}/{folderName}", file, isNeedUniqueName);
        }

        public async Task<bool> DeleteBucket(string bucketAddress)
        {
            return await _amazonS3BucketService.DeleteBucket(bucketAddress);
        }
    }
}