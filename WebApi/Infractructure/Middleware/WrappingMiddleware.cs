using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Serilog;
using WebApi.Infractructure.Helpers;
using WebApi.Infractructure.Response;

namespace WebApi.Infractructure.Middleware
{
    public class WrappingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger Log = Serilog.Log.ForContext<WrappingMiddleware>();

        public WrappingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // Do something with context near the beginning of request processing.
            var path = context.Request.Path.ToUriComponent();
            if (!path.Contains("swagger/") && !path.Contains("/invite") && (path.Contains("api") || path.Contains("admin") && !path.Contains("s3_signature")))
            {
                await RewriteResponseBody(context);
                return;
            }
            else
            {
                await _next.Invoke(context);
            }
        }

        private async Task RewriteResponseBody(HttpContext context)
        {
            var response = context.Response;
            var originalStream = response.Body;
            string bodyAsText;
            Exception exception = null;

            using (var newStream = new MemoryStream())
            {
                response.Body = newStream;
                try
                {
                    await _next.Invoke(context);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }

                response.Body = originalStream;
                newStream.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(newStream))
                {
                    bodyAsText = await reader.ReadToEndAsync();
                }
            }
            dynamic content = bodyAsText;
            try
            {
                content = JsonConvert.DeserializeObject<object>(bodyAsText);
            }
            catch {}
            
            if (exception == null)
            {
                var statusCode = (HttpStatusCode)response.StatusCode;
                if (statusCode == HttpStatusCode.NoContent) //for angular "preflight" OPTIONS request
                {
                    response.StatusCode = 200;
                }
                var apiResponse = new ApiResponse(response.StatusCode == 200, (HttpStatusCode)response.StatusCode, content, response.StatusCode == 200 ? null : content);
                var apiResponseAsString =JsonConvert.SerializeObject(apiResponse);
                // Send our modified content to the response body.
                await response.WriteAsync(apiResponseAsString);
                return;
            }
            else
            {
                await MiddlewareHelper.HandleExceptionAsync(context, exception, Log);
            }
        }
    }

    public static class WrappingExtensions
    {
        public static IApplicationBuilder UseWrapping(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<WrappingMiddleware>();
        }
    }
}