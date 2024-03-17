using DualJobDate.BusinessLogic.Services;
using Microsoft.Extensions.DependencyInjection;
using DualJobDate.BusinessLogic.Services.Interface;

namespace DualJobDate.BusinessLogic
{
    public static class ServiceRegistration
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddTransient<ITestService, TestService>();
            services.AddTransient<ICompanyService, CompanyService>();
        }

    }
}
