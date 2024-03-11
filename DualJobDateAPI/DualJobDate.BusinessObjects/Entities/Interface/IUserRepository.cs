using DualJobDate.BusinessObjects.Entities;

namespace DualJobDate.BusinessObjects.Entities.Interface
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<IEnumerable<User>> GetUsersByAcademicProgramIdAsync(int academicProgramId);
    }
}
