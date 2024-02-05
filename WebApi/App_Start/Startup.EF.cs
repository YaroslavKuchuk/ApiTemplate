using System;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace WebApi
{
    public partial class Startup
    {
        public void ConfigureEF(IServiceCollection services)
        {
            services.AddDbContextPool<AppDbContext>(options =>
                options
                    //.UseLazyLoadingProxies() // enable global lazy loading or see User entity for enable user tokens to be loaded manually
                    .UseMySql(Configuration.GetConnectionString("DefaultConnection"),
                        sqlOptions =>
                        {
                            sqlOptions
                                .ServerVersion(new Version(10, 3, 8),ServerType.MariaDb) // AWS db config
                                //.ServerVersion(new Version(5, 7), ServerType.MySql) // replace with your Server Version and Type
                                .UnicodeCharSet(CharSet.Utf8mb3)
                                .CharSetBehavior(CharSetBehavior.AppendToAllColumns)
                                .MigrationsAssembly("Migrations").MigrationsHistoryTable("EFMigrationsHistory");
                        }));
        }
    }
}
