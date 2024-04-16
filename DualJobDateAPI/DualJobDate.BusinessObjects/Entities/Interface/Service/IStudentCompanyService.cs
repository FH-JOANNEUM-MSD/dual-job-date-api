namespace DualJobDate.BusinessObjects.Entities.Interface.Service;

public interface IStudentCompanyService
{
    Task<StudentCompany?> CreateStudentCompany(bool like, int companyId, string studentId);
    Task<bool> DeleteStudentCompany(int id);
}