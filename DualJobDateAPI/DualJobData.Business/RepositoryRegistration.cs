using DualJobData.BusinessLogic.Repositories;
using DualJobData.BusinessLogic.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DualJobData.BusinessLogic
{
    public static class RepositoryRegistration
    {
        public static void RegisterRepository(IServiceCollection services)
        {
            services.AddTransient<IUserRepository, UserRepository>();
        }
    }
}
