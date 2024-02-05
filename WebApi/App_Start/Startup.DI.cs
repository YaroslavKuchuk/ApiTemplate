using Core.Data;
using Core.IdentityEntities;
using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Services.IdentityServices;
using Services.IdentityServices.Interfaces;
using Services.Implementation;
using Services.Implementation.ImportExportData;
using Services.Implementation.Message;
using Services.Implementation.Notifications;
using Services.Interfaces;
using Services.Interfaces.ImportExportData;
using Services.Interfaces.Notifications;
using WebApi.Infractructure.Authorization;

namespace WebApi
{
    public partial class Startup
    {
        public void ConfigureDI(IServiceCollection services)
        {
            services.AddScoped<IContext, AppDbContext>(); //Scoped lifetime services are created once per request.
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IAppUserManager, AppUserManager>();
            services.AddScoped<RoleManager<AppRole>>();

            services.AddScoped<IQueueMessageService, QueueMessageService>();
            services.AddScoped<IUserDeviceService, UserDeviceService>();
            services.AddScoped<IUserTokenService, UserTokenService>();
            services.AddScoped<ISettingService, SettingService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IMediaService, MediaService>();
            services.AddScoped<IActivationCodeService, ActivationCodeService>();
            services.AddScoped<IForgotPasswordService, ForgotPasswordService>();
            services.AddScoped<IImportExportService, ImportExportService>(); 

            // Notification services
            services.AddScoped<IEmailNotificationService, EmailNotificationService>();
            services.AddScoped<IPushNotificationService, PushNotificationService>();
            services.AddScoped<IMessageHistoryService, MessageHistoryService>();

            // Add Database Initializer
            services.AddScoped<IDbInitializerService, DbInitializerService>();

            services.AddScoped<IAuthorizationHandler, IsUserValidHandler>();
            services.AddScoped<IAuthorizationHandler, PermissionHandler>();
        }
    }
}
