using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using WebApi.Infractructure.Response;

namespace WebApi.Filters
{
    /// <summary>
    /// Auto model validation
    /// </summary>
    /// <seealso cref="IAsyncActionFilter" />
    public class ModelValidationFilterAttribute : IAsyncActionFilter
    {
        private IEnumerable<string> GetErrors(ActionExecutingContext actionContext)
        {
            var result = new List<string>();
            foreach (var state in actionContext.ModelState)
            {
                result.AddRange(state.Value.Errors.Select(e => e.ErrorMessage));
            }
            return result;
        }
        private async Task HandleExceptionAsync(HttpContext context, string errors)
        {
            var response = context.Response;
            var status = HttpStatusCode.Conflict;
            var responseValue = new ApiResponse(false, status, null, errors);
            var apiResponseAsString = JsonConvert.SerializeObject(responseValue);
            await response.WriteAsync(apiResponseAsString);
            return;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = GetErrors(context);
                throw new ArgumentException(string.Join("\n ", errors));
                //await HandleExceptionAsync(context.HttpContext, string.Join("\n", errors));
            }

            await next();
        }
    }
}