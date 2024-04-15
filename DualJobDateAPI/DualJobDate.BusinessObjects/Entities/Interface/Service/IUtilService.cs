namespace DualJobDate.BusinessObjects.Entities.Interface.Service;

public interface IUtilService
{
    Task<IQueryable<Institution>> GetInstitutionsAsync();
    Task<IQueryable<AcademicProgram>> GetAcademicProgramsAsync();
}