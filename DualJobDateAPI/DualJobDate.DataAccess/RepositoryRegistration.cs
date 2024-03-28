using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface;
using DualJobDate.BusinessObjects.Entities.Interface.Repository;
using DualJobDate.BusinessObjects.Entities.Models;
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
            services.AddScoped<IInstitutionRepository, InstitutionRepository>();
            services.AddScoped<IAcademicProgramRepository, AcademicProgramRepository>();
            services.AddScoped<IAcademicDegreeRepository, AcademicDegreeRepository>();
            services.AddScoped<IActivityRepository, ActivityRepository>();
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<ICompanyDetailsRepository, CompanyDetailsRepository>();
            services.AddScoped<ICompanyActivityRepository, CompanyActivityRepository>();
        }
    }
}
