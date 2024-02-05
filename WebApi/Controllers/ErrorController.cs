using System.Net;
using Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Infractructure.Response;

namespace WebApi.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/[controller]")]
    public class ErrorController : Controller
    {
        [AllowAnonymous]
        [HttpGet("/Error"), HttpPost("/Error")]
        [ProducesResponseType(typeof(ResponseWrapper<ApiResponse>), 200)]
        public string HandleError([FromQuery] int status = 400)
        {
            var errorMessage = "";
            switch (status)
            {
                case 401:
                    errorMessage = "Please login with proper credentials";
                    break;
                case 403:
                    errorMessage = "Sorry, but you do not currently have permissions to use this resourсe";
                    break;
                case 404:
                    errorMessage = "Sorry, requested resourсe was not found";
                    //return new ApiResponse(false, (HttpStatusCode)status, null, errorMessage);
                    return errorMessage;
                case 500:
                    errorMessage = "Error has occurred while processing the request";
                    throw new ApiException(errorMessage, (HttpStatusCode)status);
                default:
                    errorMessage = "Error has occurred while processing the request";
                    return errorMessage;
            }
            return errorMessage;
        }
    }
}