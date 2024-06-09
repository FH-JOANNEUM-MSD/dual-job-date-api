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
        const string jwt = "JwtDebug";
#else
        const string jwt = "JwtRelease";
#endif
        services.AddTransient<ITestService, TestService>();
        services.AddTransient<ICompanyService, CompanyService>();
        services.AddTransient<IEmailHelper, EmailHelper>();
        _ = services.AddTransient<IJwtAuthManager, JwtAuthManager>(x =>
        {
            var userManager = x.GetRequiredService<UserManager<User>>();
            var secret = Encoding.ASCII.GetBytes(configuration[$"{jwt}:JwtSecret"]) ?? throw new NotImplementedException("JwtSecret is not defined");
            var issuer = configuration[$"{jwt}:Audience"] ?? throw new NotImplementedException("Audience is not defined");
            var audience = configuration[$"{jwt}:Issuer"] ?? throw new NotImplementedException("Issuer is not defined");
            var expireDate1 = short.Parse(configuration[$"{jwt}:AccessTokenExpiration"] ?? throw new NotImplementedException("AccessTokenExpiration is not defined"));
            var expireDate2 = short.Parse(configuration[$"{jwt}:RefreshTokenExpiration"] ?? throw new NotImplementedException("RefreshTokenExpiration is not defined"));
            return new JwtAuthManager(userManager, secret, issuer, audience, expireDate1, expireDate2);
        });
        services.AddTransient<IStudentCompanyService, StudentCompanyService>();
        services.AddTransient<IUtilService, UtilService>();
    }
}