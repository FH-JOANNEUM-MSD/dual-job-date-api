using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface;

namespace DualJobDate.DataAccess.Repositories
{
    public class UserRepository(AppDbContext dbContext) : BaseRepository<User>(dbContext), IUserRepository
    {
        public async Task<IEnumerable<User>> GetUsersByAcademicProgramIdAsync(int academicProgramId)
        {
            var users = await GetAllAsync();
            return users.Where(x => x.AcademicProgramId == academicProgramId);
        }
    }
}
