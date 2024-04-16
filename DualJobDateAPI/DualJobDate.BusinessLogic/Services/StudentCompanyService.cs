using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface;
using DualJobDate.BusinessObjects.Entities.Interface.Service;
using Microsoft.AspNetCore.Mvc;
using Mysqlx;

namespace DualJobDate.BusinessLogic.Services;

public class StudentCompanyService(IUnitOfWork unitOfWork) : IStudentCompanyService
{
    public async Task<StudentCompany?> CreateStudentCompany(bool like, int companyId, string studentId)
    {
        var studentCompany = (await unitOfWork.StudentCompanyRepository.GetAllAsync()).FirstOrDefault(x => x.StudentId == studentId && x.CompanyId == companyId);

        try
        {
            unitOfWork.BeginTransaction();
            if (studentCompany == null)
            {
                studentCompany = new StudentCompany()
                {
                    CompanyId = companyId,
                    Like = like,
                    StudentId = studentId
                };

                await unitOfWork.StudentCompanyRepository.AddAsync(studentCompany);
            }
            else
            {
                studentCompany.Like = like;
                await unitOfWork.StudentCompanyRepository.UpdateAsync(studentCompany);
            }

            unitOfWork.Commit();
            await unitOfWork.SaveChanges();
            return studentCompany;
        }
        catch
        {
            return null;
        }
        
    }
    
    public async Task<bool> DeleteStudentCompany(int id)
    {
        try
        {
            unitOfWork.BeginTransaction();
            await unitOfWork.StudentCompanyRepository.DeleteAsync(id);
            unitOfWork.Commit();
            await unitOfWork.SaveChanges();
            return true;
        }
        catch
        {
            return false;
        }
        
    }
}