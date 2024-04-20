using DualJobDate.BusinessObjects.Entities.Models;

namespace DualJobDate.BusinessObjects.Entities.Interface.Service;

public interface IUtilService
{
    Task<IQueryable<Institution>> GetInstitutionsAsync();
    Task<IQueryable<AcademicProgram>> GetAcademicProgramsAsync();
    Task<AcademicProgram?> PostAcademicProgramAsync(int id,AcademicProgramModel model);
    Task<Institution?> PostInstitutionAsync(InstitutionModel model);
}