using DualJobData.BusinessLogic.Entities;
using DualJobData.BusinessLogic.Repositories.Interfaces;
using DualJobData.DataAccess;

namespace DualJobData.BusinessLogic.Repositories
{
    public class UserRepository(AppDbContext context) : BaseRepository<User>(context), IUserRepository
    {
        public IEnumerable<User> GetUsersByStation()
        {
            var users = base.GetAll();
            return [.. users];
        }
    }
}
