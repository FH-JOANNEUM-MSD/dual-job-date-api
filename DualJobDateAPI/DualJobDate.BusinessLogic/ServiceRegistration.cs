using DualJobDate.BusinessObjects.Entities.Interface;
using DualJobDate.BusinessLogic.Services;
using Microsoft.Extensions.DependencyInjection;
using DualJobDate.BusinessLogic.Helper;
using Microsoft.Extensions.Configuration;

namespace DualJobDate.BusinessLogic
{
    public static class ServiceRegistration
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddTransient<ITestService, TestService>();
            services.AddTransient<IEmailSender, EmailSender>();
            _ = services.AddTransient<IJwtHelper, JwtHelper>(x =>
            {
                var configuration = x.GetRequiredService<IConfiguration>();
                var secretKey = configuration["JwtSecret"] ?? throw new NotImplementedException("JwtSecret is not defined");
                return new JwtHelper(secretKey);
            });
        }

    }
}
