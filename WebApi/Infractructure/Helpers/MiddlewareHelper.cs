using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Serilog;
using WebApi.Infractructure.Response;

namespace WebApi.Infractructure.Helpers
{
    public static class MiddlewareHelper
    {
        public static async Task HandleExceptionAsync(HttpContext context, Exception exception, ILogger log)
        {
            var status = HttpStatusCode.InternalServerError;
            string message;
            var response = context.Response;
            var userId = Convert.ToInt64(context.Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "sid")?.Value);

            var exceptionType = exception.GetType();
            if (typeof(ApiException).IsAssignableFrom(exceptionType))
            {
                var apiException = (ApiException)exception;
                message = apiException.Message;
                status = apiException.StatusCode;
            }
            else if (exceptionType == typeof(UnauthorizedAccessException))
            {
                var apiException = (ApiException)exception;
                message = apiException.Message;
                status = apiException.StatusCode;
            }
            else if (exceptionType == typeof(NotImplementedException))
            {
                message = "A server error occurred";
                status = HttpStatusCode.NotImplemented;
                Log.ForContext("UserId", userId != 0 ? userId : (object)null).Error(exception, "An {@Exception} has been occurred");
            }
            else if (exceptionType == typeof(ArgumentException))
            {
                message = exception.Message;
                status = HttpStatusCode.Conflict;
            }
            else
            {
                message = exception.Message;
                Log.ForContext("UserId", userId != 0 ? userId : (object)null).Error(exception, "An {@Exception} has been occurred");
            }

            var responseValue = new ApiResponse(false, status, null, message);
            var apiResponseAsString = JsonConvert.SerializeObject(responseValue);
            response.StatusCode = (int)status;
            await context.Response.WriteAsync(apiResponseAsString);
            return;
        }
    }
}
