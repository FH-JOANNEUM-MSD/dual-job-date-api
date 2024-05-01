using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface;
using DualJobDate.BusinessObjects.Entities.Interface.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mysqlx;

namespace DualJobDate.BusinessLogic.Services;

public class StudentCompanyService(IUnitOfWork unitOfWork) : IStudentCompanyService
{
    public async Task<List<StudentCompany>> GetStudentCompaniesAsync(int id)
    {
        var result = await unitOfWork.StudentCompanyRepository.GetAllAsync().Result.Where(s => s.Student.AcademicProgramId == id).ToListAsync();
        return result;
    }
    
    public async Task<List<StudentCompany>> GetStudentCompaniesByStudentIdAsync(string studentId)
    {
        var result = (await unitOfWork.StudentCompanyRepository.GetAllAsync()).Where(x => x.StudentId == studentId);
        return await result.ToListAsync();
    }
    
    public async Task<StudentCompany?> GetStudentCompanyByIdAsync(int id)
    {
        return await (await unitOfWork.StudentCompanyRepository.GetAllAsync()).FirstOrDefaultAsync(x => x.Id == id);
    }
    
    public async Task<StudentCompany?> CreateStudentCompanyAsync(bool like, int companyId, string studentId)
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
    
    public async Task<bool> DeleteStudentCompanyAsync(int id)
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