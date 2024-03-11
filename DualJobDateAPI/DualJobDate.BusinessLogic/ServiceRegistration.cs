using DualJobDate.BusinessObjects.Entities.Interface;
using DualJobDate.BusinessLogic.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DualJobDate.BusinessLogic
{
    public static class ServiceRegistration
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddTransient<ITestService, TestService>();
        }

    }
}
