namespace DualJobDate.BusinessObjects.Entities.Interface.Service;

public interface IStudentCompanyService
{
    Task<List<StudentCompany>> GetStudentCompaniesAsync();
    Task<List<StudentCompany>> GetStudentCompaniesByStudentIdAsync(string studentId);
    Task<StudentCompany?> GetStudentCompanyByIdAsync(int id);
    Task<StudentCompany?> CreateStudentCompanyAsync(bool like, int companyId, string studentId);
    Task<bool> DeleteStudentCompanyAsync(int id);
    Dictionary<User, List<Company>> MatchCompaniesToStudents(List<User> students, List<Company> companies,
        int matchesPerStudent = 6);
}