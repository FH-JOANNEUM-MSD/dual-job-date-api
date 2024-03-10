using DualJobData.BusinessLogic.Entities;
using DualJobData.BusinessLogic.Repositories.Interfaces;
using DualJobData.DataAccess;

namespace DualJobData.BusinessLogic.Repositories
{
    public class UserRepository(AppDbContext dbContext) : BaseRepository<User>(dbContext), IUserRepository
    {
        public async Task<IEnumerable<User>> GetUsersByAcademicProgramIdAsync(int academicProgramId)
        {
            var users = await base.GetAllAsync();
            return users.Where(x => x.AcademicProgramId == academicProgramId);
        }
    }
}
