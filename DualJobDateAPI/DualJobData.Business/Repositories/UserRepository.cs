using DualJobData.BusinessLogic.Entities;
using DualJobData.DataAccess;
using DualJobDateAPI.Repository.Interfaces;

namespace DualJobDateAPI.Repository
{
    public class UserRepository : BaseRepository<User>
    {
        public UserRepository(AppDbContext context, StationTenantConfig stationTenantConfig) : base(context, stationTenantConfig)
        {
        }

        public IEnumerable<User> GetUsersByStation()
        {
            var users = base.GetAll()
                .Where(x => x.StationId == _stationId);

            return users.ToList();
        }
    }
}
