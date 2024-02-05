using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.IdentityServices.Enum;
using Services.Interfaces;
using Services.Model;
using Services.Model.Content;
using WebApi.Infractructure.Response;

namespace WebApi.Controllers
{
    /// <summary>
    /// Upload image to AWS S3 Bucket
    /// </summary>
    /// <seealso cref="BaseApiController" />
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class UploadController : BaseApiController
	{
        private readonly IMediaService _mediaService;

		/// <summary>
		/// Initializes a new instance of the <see cref="UploadController" /> class.
		/// </summary>
		public UploadController(IMediaService mediaService)
		{
		    _mediaService = mediaService;
		}

        /// <summary>
        /// Upload given base64 file to AWS S3 Bucket
        /// </summary>
        /// <returns>URL to file from S3 bucket</returns>
        [HttpPost]
        [Route("base64")]
        [ProducesResponseType(typeof(ResponseWrapper<UrlModel>), 200)]
        [Authorize(Policy = PermissionString.HasUploadFile)]
        public async Task<UrlModel> UploadFile([FromBody] Base64FileModel model)
        {
            var url = await _mediaService.UploadFileToBucketFromBase64(model, UserId);
            return new UrlModel { FileUrl = url };
        }

	    /// <summary>
	    /// Upload given file to AWS S3 Bucket
	    /// </summary>
	    /// <returns>URL to file from S3 bucket</returns>
	    [HttpPost]
	    [Route("file")]
        [ProducesResponseType(typeof(ResponseWrapper<UrlModel>), 200)]
        [Authorize(Policy = PermissionString.HasUploadFile)]
        public async Task<UrlModel> Upload(IFormFile file, FolderModel folderModel = null)
	    {
	        var url = await _mediaService.UploadFileToBucketFromFile(file, UserId, folderModel?.FolderName);
	        return new UrlModel { FileUrl = url };
	    }
    }
}