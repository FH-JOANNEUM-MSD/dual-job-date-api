namespace DualJobDate.BusinessObjects.Entities.Interface.Repository;

public interface IAcademicProgramRepository : IBaseRepository<AcademicProgram>
{
    Task<AcademicProgram> GetByName(string KeyName);
}