using DualJobDate.BusinessObjects.Entities.Models;

namespace DualJobDate.BusinessObjects.Entities.Interface.Service;

public interface IUtilService
{
    Task<IQueryable<Institution>> GetInstitutionsAsync();
    Task<Institution> GetInstitutionByKeyNameAsync(string keyName);
    Task<IQueryable<AcademicProgram>> GetAcademicProgramsAsync();
    Task<AcademicProgram> GetAcademicProgramByKeyNameAndYearAsync(string keyName, int year);
    Task<AcademicProgram?> PostAcademicProgramAsync(int id,AcademicProgramModel model);
    Task<Institution?> PostInstitutionAsync(InstitutionModel model);
    Task<Company?> PutCompanyAsync(string name, int academicProgramId, string userId);
}