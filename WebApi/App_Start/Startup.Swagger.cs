using System;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using WebApi.Filters;

namespace WebApi
{
    public partial class Startup
    {

        public void ConfigureSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.DocumentFilter<LowercaseDocumentFilter>();
                c.DescribeAllEnumsAsStrings();
                c.DescribeStringEnumsInCamelCase();
                c.OperationFilter<SwaggerAuthorizationHeaderFilter>();
                c.OperationFilter<FormFileSwaggerFilter>();
                //Entity xml documentation
                var xmlPath = $@"{AppDomain.CurrentDomain.BaseDirectory}WebApiTemplate.xml";
                c.IncludeXmlComments(xmlPath);
                c.SwaggerDoc("v1", new Info { Title = "Web Api", Version = "v1" });
            });
        }
    }
}
