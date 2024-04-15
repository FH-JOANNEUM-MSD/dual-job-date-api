namespace DualJobDate.BusinessObjects.Entities.Interface.Service;

public interface IStudentCompanyService
{
    Task<StudentCompany?> AddStudentCompany(bool like, int companyId, string studentId);
}