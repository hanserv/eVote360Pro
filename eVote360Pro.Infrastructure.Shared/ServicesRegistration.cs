using eVote360Pro.Core.Application.Interfaces;
using eVote360Pro.Core.Domain.Settings;
using eVote360Pro.Infrastructure.Shared.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eVote360Pro.Infrastructure.Shared
{
    public static class ServicesRegistration
    {
        public static void AddSharedLayer(this IServiceCollection services, IConfiguration configuration)
        {
            #region Configurations
            services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
            #endregion

            #region Services
            services.AddScoped<IOcrService,TesseractOcrService>();
            services.AddScoped<IEmailService,EmailService>();
            #endregion 
        }
    }
}
