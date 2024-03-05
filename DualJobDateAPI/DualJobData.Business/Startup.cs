using DualJobData.BusinessLogic.Entities;
using DualJobDateAPI.Repository;
using DualJobDateAPI.Repository.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DualJobData.BusinessLogic
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            //TODO add DbContext Injection
            services.AddTransient<IBaseRepository<User>, UserRepository>();
        }
    }
}
