using DualJobData.BusinessLogic.Services.Interface;
using DualJobData.BusinessLogic.Services;

namespace DualJobDate.API
{
    public static class ServiceRegistration
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddTransient<ITestService, TestService>();
        }

    }
}
