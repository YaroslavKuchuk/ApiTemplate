using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Services.Model.Content;

namespace Services.Interfaces
{
    public interface IMediaService
    {
        Task<string> UploadFileToBucketFromBase64(Base64FileModel model, long userId, string folderName = null, bool isNeedUniqueName = true);
        Task<string> UploadFileToBucketFromFile(IFormFile file, long userId, string folderName = null, bool isNeedUniqueName = true);
        Task<bool> DeleteBucket(string bucketAddress);
    }
}
