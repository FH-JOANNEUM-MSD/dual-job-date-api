namespace DualJobDate.BusinessObjects.Entities.Interface.Repository;

public interface IAcademicProgramRepository : IBaseRepository<AcademicProgram>
{
    Task<AcademicProgram> GetByNameAndYear(string KeyName, int Year);
    Task<IQueryable<AcademicProgram>> GetByInstitutionId(int institutionId);
}