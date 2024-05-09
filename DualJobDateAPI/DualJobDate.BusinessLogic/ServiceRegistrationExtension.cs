using DualJobDate.BusinessLogic.Services;
using DualJobDate.BusinessObjects.Entities.Interface.Service;
using Microsoft.Extensions.DependencyInjection;

namespace DualJobDate.BusinessLogic;

public static class ServiceRegistrationExtension
{
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddTransient<ITestService, TestService>();
        services.AddTransient<ICompanyService, CompanyService>();
        services.AddTransient<ICompanyService, CompanyService>();
        services.AddTransient<IStudentCompanyService, StudentCompanyService>();
        services.AddTransient<IUtilService, UtilService>();
    }
}