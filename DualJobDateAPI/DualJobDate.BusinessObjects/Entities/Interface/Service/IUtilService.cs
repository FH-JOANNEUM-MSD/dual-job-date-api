namespace DualJobDate.BusinessObjects.Entities.Interface.Service;

public interface IUtilService
{
    Task<IQueryable<Institution>> GetInstitutionsAsync();
    Task<IQueryable<AcademicProgram>> GetAcademicProgramsAsync();
    Task PostAcademicProgramAsync(AcademicProgram academicProgram);
}