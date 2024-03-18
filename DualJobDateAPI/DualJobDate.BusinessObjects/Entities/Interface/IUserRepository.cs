using DualJobDate.BusinessObjects.Entities.Interface.Repository;

namespace DualJobDate.BusinessObjects.Entities.Interface
{
    public interface IUserRepository : IBaseRepository<ApplicationUser>
    {
        Task<IEnumerable<ApplicationUser>> GetUsersByAcademicProgramIdAsync(int academicProgramId);
    }
}
