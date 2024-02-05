using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services.Model.Mappings;
using WebApi.Infractructure.Middleware;

namespace WebApi
{
    public partial class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureEF(services);
            ConfigureAuthentication(services);
            ConfigureDI(services);
            ConfigureWebApi(services);
            AutoMapperConfig.Register();
            ConfigureSwagger(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            app.UseResponseCompression();

            app.UseErrorHandling();
            app.UseWrapping();

            app.UseStaticFiles();
            app.UseStatusCodePages();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseAuthentication();

            var allowedOrigins = Configuration.GetSection("AllowedOrigins:Origins").Value.Split(',');
            app.UseCors(builder => builder.WithOrigins(allowedOrigins)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowCredentials());

            app.UseSwagger();
            var swaggerEndpoint = "swagger";
            app.UseSwaggerUI(c =>
            {
                c.DocExpansion("none");
                c.DocumentTitle("Web Api");
                c.SwaggerEndpoint($"/{swaggerEndpoint}/v1/swagger.json", "Web Api");
            });
            app.UseStatusCodePagesWithReExecute("/Error", "?status={0}");

           // app.UseSerilogTracking(); // use this for tracking HTTP-requests and time measuring

            var option = new RewriteOptions();
            option.AddRedirect("^$", swaggerEndpoint);
            app.UseRewriter(option); //redirect root url to swagger endpoint

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "DefaultApi",
                    "api/{controller}/{id?}");
                routes.MapRoute(
                    "Admin",
                    "admin/{controller}/{id?}");
            });
        }
    }
}