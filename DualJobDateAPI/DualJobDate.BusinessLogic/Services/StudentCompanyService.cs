using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface;
using DualJobDate.BusinessObjects.Entities.Interface.Service;

namespace DualJobDate.BusinessLogic.Services;

public class StudentCompanyService(IUnitOfWork unitOfWork) : IStudentCompanyService
{
    public async Task<StudentCompany?> AddStudentCompany(bool like, int companyId, string studentId)
    {
        unitOfWork.BeginTransaction();
        var studentCompany = new StudentCompany()
        {
            CompanyId = companyId,
            Like = like,
            StudentId = studentId
        };
        await unitOfWork.StudentCompanyRepository.AddAsync(studentCompany);
        unitOfWork.Commit();
        await unitOfWork.SaveChanges();
        return studentCompany;
    }
}