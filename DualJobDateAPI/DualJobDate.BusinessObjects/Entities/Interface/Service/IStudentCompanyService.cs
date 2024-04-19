namespace DualJobDate.BusinessObjects.Entities.Interface.Service;

public interface IStudentCompanyService
{
    Task<List<StudentCompany>> GetStudentCompaniesAsync();
    Task<List<StudentCompany>> GetStudentCompaniesByStudentIdAsync(string studentId);
    Task<StudentCompany?> CreateStudentCompanyAsync(bool like, int companyId, string studentId);
    Task<bool> DeleteStudentCompanyAsync(int id);
}