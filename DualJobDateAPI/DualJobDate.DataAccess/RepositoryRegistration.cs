using DualJobDate.BusinessObjects.Entities.Interface;
using DualJobDate.BusinessObjects.Entities.Interface.Repository;
using DualJobDate.DataAccess.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DualJobDate.DataAccess
{
    public static class RepositoryRegistration
    {
        public static void RegisterRepository(IServiceCollection services)
        {
            services.AddScoped<IDbContext, AppDbContext>();
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IInstitutionRepository, InstitutionRepository>();
        }
    }
}
