using DualJobData.BusinessLogic.Repositories;
using DualJobData.BusinessLogic.Repositories.Interfaces;
using DualJobData.BusinessLogic.UnitOfWork;
using DualJobData.DataAccess;
using Microsoft.Extensions.DependencyInjection;

namespace DualJobData.BusinessLogic
{
    public static class RepositoryRegistration
    {
        public static void RegisterRepository(IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddScoped<IUserRepository, UserRepository>();
        }
    }
}
