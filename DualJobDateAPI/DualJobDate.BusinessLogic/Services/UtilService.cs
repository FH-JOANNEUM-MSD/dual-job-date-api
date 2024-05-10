using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface;
using DualJobDate.BusinessObjects.Entities.Interface.Service;
using DualJobDate.BusinessObjects.Entities.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DualJobDate.BusinessLogic.Services;

public class UtilService(IUnitOfWork unitOfWork, UserManager<User> userManager) : IUtilService
{
    public Task<IQueryable<Institution>> GetInstitutionsAsync()
    {
        var ret = unitOfWork.InstitutionRepository.GetAllAsync();
        return ret;
    }
    
    public Task<Institution> GetInstitutionByKeyNameAsync(string keyName)
    {
        var ret = unitOfWork.InstitutionRepository.GetByName(keyName);
        return ret;
    }

    public async Task<IQueryable<AcademicProgram>> GetAcademicProgramsAsync(int? institutionId)
    {
        var ret = await unitOfWork.AcademicProgramRepository.GetAllAsync();
        if (institutionId is not null)
        {
            ret = ret.Where(ap => ap.InstitutionId == institutionId);
        }

        return ret;
    }
    
    public Task<AcademicProgram> GetAcademicProgramByKeyNameAndYearAsync(string keyName, int year)
    {
        var ret = unitOfWork.AcademicProgramRepository.GetByNameAndYear(keyName, year);
        return ret;
    }

    public async Task<AcademicProgram?> PostAcademicProgramAsync(int id, AcademicProgramModel model)
    {
        var ac = await unitOfWork.AcademicProgramRepository.GetByNameAndYear(model.KeyName, model.Year);
        if (ac != null)
        {
            throw new ArgumentException("AcademicProgram with same KeyName and Year already exists!");
        }
        unitOfWork.BeginTransaction();
        var academicProgram = new AcademicProgram
        {
            InstitutionId = id,
            Year = model.Year,
            Name = model.Name,
            KeyName = model.KeyName,
            AcademicDegreeEnum = model.AcademicDegreeEnum
        };
        await unitOfWork.AcademicProgramRepository.AddAsync(academicProgram);
        unitOfWork.Commit();
        await unitOfWork.SaveChanges();
        return academicProgram;
    }
    
    public async Task<Institution?> PostInstitutionAsync(InstitutionModel model)
    {
        var i = await unitOfWork.InstitutionRepository.GetByName(model.KeyName);
        if (i != null)
        {
            throw new ArgumentException("Institution already exists!");
        }
        unitOfWork.BeginTransaction();
        var institution = new Institution
        {
            Name = model.Name,
            KeyName = model.KeyName,
            Website = model.Website
        };
        await unitOfWork.InstitutionRepository.AddAsync(institution);
        unitOfWork.Commit();
        await unitOfWork.SaveChanges();
        return institution;
    }
    
    public async Task<Company?> PutCompanyAsync(string name, int academicProgramId, int institutionId, string userId)
    {
        var i = await (await unitOfWork.CompanyRepository.GetAllAsync()).Where(x => x.Name == name && x.AcademicProgramId == academicProgramId && x.InstitutionId == institutionId).FirstOrDefaultAsync();
        if (i != null)
        {
            throw new ArgumentException("Company already exists!");
        }
        unitOfWork.BeginTransaction();
        var company = new Company()
        {
            Name = name,
            AcademicProgramId = academicProgramId,
            InstitutionId = institutionId,
            UserId = userId
        };
        await unitOfWork.CompanyRepository.AddAsync(company);
        unitOfWork.Commit();
        await unitOfWork.SaveChanges();
        return company;
    }

    public async Task<IQueryable<Appointment>> GetAppointmentsByUserIdAsync(string userId)
    {
        var ret = await unitOfWork.AppointmentRepository.GetAllAsync();
        if (ret.Count(x => x.User.Id == userId) == 0)
        {
            throw new ArgumentException("No appointments found for this user!");
        }
        var user = await userManager.FindByIdAsync(userId);
        var list = ret.Where(x => x.User.Id == userId);
        return list;
    }

    public async Task<IQueryable<Appointment>> GetAppointmentsByCompanyIdAsync(int companyId)
    {
        var ret = await unitOfWork.AppointmentRepository.GetAllAsync();
        if (ret.Count(x => x.Company.Id == companyId) == 0)
        {
            throw new ArgumentException("No appointments found for this company!");
        }
        return ret.Where(x => x.Company.Id == companyId);
    }
}