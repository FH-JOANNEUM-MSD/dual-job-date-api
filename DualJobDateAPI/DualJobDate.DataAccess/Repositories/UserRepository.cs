using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface;

namespace DualJobDate.DataAccess.Repositories
{
    public class UserRepository(AppDbContext dbContext) : BaseRepository<ApplicationUser>(dbContext), IUserRepository
    {
        public async Task<IEnumerable<ApplicationUser>> GetUsersByAcademicProgramIdAsync(int academicProgramId)
        {
            var users = await GetAllAsync();
            return users.Where(x => x.AcademicProgramId == academicProgramId);
        }
    }
}
