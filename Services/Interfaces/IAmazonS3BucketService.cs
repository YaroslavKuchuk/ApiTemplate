using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Services.Interfaces
{
    public interface IAmazonS3BucketService
    {
        Task<string> UploadFile(string fileSource, string fileName, string fileData, string contentType, bool isNeedUniqueName = true);
        Task<string> UploadSingleFile(string fileSource, IFormFile file, bool isNeedUniqueName = true);
        Task<bool> DeleteBucket(string keyName);
    }
}
