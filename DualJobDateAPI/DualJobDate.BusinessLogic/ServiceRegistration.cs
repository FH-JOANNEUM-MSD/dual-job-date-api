using System.Text;
using DualJobDate.BusinessLogic.Helper;
using DualJobDate.BusinessLogic.Services;
using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface.Helper;
using DualJobDate.BusinessObjects.Entities.Interface.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DualJobDate.BusinessLogic;

public static class ServiceRegistration
{
    public static void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
#if DEBUG
        string Jwt = "JwtDebug";
#else
        string Jwt = "JwtRelease";
#endif
        services.AddTransient<ITestService, TestService>();
        services.AddTransient<ICompanyService, CompanyService>();
        services.AddTransient<IEmailHelper, EmailHelper>();
        _ = services.AddTransient<IJwtAuthManager, JwtAuthManager>(x =>
        {
            var userManager = x.GetRequiredService<UserManager<User>>();
            var secret = Encoding.ASCII.GetBytes(configuration[$"{Jwt}:JwtSecret"]) ?? throw new NotImplementedException("JwtSecret is not defined");
            var issuer = configuration[$"{Jwt}:Audience"] ?? throw new NotImplementedException("Audience is not defined");
            var  audience = configuration[$"{Jwt}:Issuer"] ?? throw new NotImplementedException("Issuer is not defined");
            return new JwtAuthManager(userManager, secret, issuer, audience);
        });
        services.AddTransient<ICompanyService, CompanyService>();
    }
}