using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Services.Interfaces;

namespace WebApi
{
    public class Program
    {
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .Enrich.FromLogContext()
                //.Enrich.WithExceptionDetails()
                .WriteTo.Async(l => l.Map(le => le.Level, (level, wt) => wt.RollingFile($"./logs/{ DateTime.UtcNow.Date:dd.MM.yyyy}/log-{level}-{{Date}}.txt", outputTemplate: "{Timestamp:dd-MM-yyyy HH:mm:ss.fff} [{Level}] |{SourceContext}.{Method}| {Message}{NewLine}", shared:true)))
                .WriteTo.MySQL(Configuration.GetConnectionString("DefaultConnection"), "Serilog",  restrictedToMinimumLevel: LogEventLevel.Error, storeTimestampInUtc:true)
                .CreateLogger();
            try
            {
                Log.Information("Starting web host");
                var host = CreateWebHostBuilder(args).Build();
                using (var scope = host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    try
                    {
                        //Generate EF Core Seed Data
                        var dbInitializer = services.GetRequiredService<IDbInitializerService>();
                        dbInitializer.Initialize();
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "An error occurred while seeding the DB.");
                    }
                }

                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseKestrel()
                .UseSockets()
                //.UseContentRoot(Directory.GetCurrentDirectory())
                //.UseIISIntegration()
                .UseStartup<Startup>()
                .UseSerilog();
    }
}
