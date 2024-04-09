using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface;
using DualJobDate.BusinessObjects.Entities.Interface.Service;
using DualJobDate.BusinessObjects.Entities.Models;
using DualJobDate.BusinessObjects.Resources;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DualJobDate.BusinessLogic.Services;

public class CompanyService(IUnitOfWork unitOfWork, UserManager<User> userManager) : ICompanyService
{
    public Task<Company?> GetCompanyByIdAsync(int id)
    {
        var result = unitOfWork.CompanyRepository.GetByIdAsync(id);
        return result;
    }

    public Task<List<Company>> GetActiveCompaniesAsync(int academicProgramId)
    {
        var result = unitOfWork.CompanyRepository.GetAllAsync();
        return result.Result.Where(c => c.AcademicProgramId == academicProgramId && c.IsActive == true)
            .ToListAsync();
    }

    public Task<List<Company>> GetCompaniesByInstitutionAsync(int institutionId)
    {
        var result = unitOfWork.CompanyRepository.GetAllAsync();
        return result.Result.Where(c => c.InstitutionId == institutionId).ToListAsync();
    }

    public Task<List<Company>> GetCompaniesByAcademicProgramAsync(int academicProgramId)
    {
        var result = unitOfWork.CompanyRepository.GetAllAsync();
        return result.Result.Where(c => c.AcademicProgramId == academicProgramId).ToListAsync();
    }

    public async Task UpdateCompany(UpdateCompanyModel model, Company company)
    {
        unitOfWork.BeginTransaction();
        company.Industry = model.Industry;
        company.LogoBase64 = model.LogoBase64;
        company.Website = model.Website;
        await unitOfWork.CompanyRepository.UpdateAsync(company);
        unitOfWork.Commit();
        await unitOfWork.SaveChanges();
    }

    public async Task UpdateCompanyActivity(bool isActive, Company company)
    {
        unitOfWork.BeginTransaction();
        company.IsActive = isActive;
        await unitOfWork.CompanyRepository.UpdateAsync(company);
        unitOfWork.Commit();
        await unitOfWork.SaveChanges();
    }

    public async Task<CompanyDetails?> GetCompanyDetailsAsync(Company company)
    {
        if (company.CompanyDetailsId == null)
            return null;
        return await unitOfWork.CompanyDetailsRepository.GetByIdAsync((int)company.CompanyDetailsId);
    }

    public async Task UpdateCompanyDetails(CompanyDetails details, Company company)
    {
        unitOfWork.BeginTransaction();
        if (company.CompanyDetailsId == null)
        {
            company.CompanyDetails = details;
            await unitOfWork.CompanyDetailsRepository.AddAsync(company.CompanyDetails);
            await unitOfWork.CompanyRepository.UpdateAsync(company);
        }
        else
        {
            var companyDetails = await unitOfWork.CompanyDetailsRepository.GetByIdAsync((int)company.CompanyDetailsId);
            companyDetails.Trainer = details.Trainer;
            companyDetails.ContactPersonInCompany = details.ContactPersonInCompany;
            companyDetails.JobDescription = details.JobDescription;
            companyDetails.ShortDescription = details.ShortDescription;
            companyDetails.TrainerPosition = details.TrainerPosition;
            companyDetails.TrainerTraining = details.TrainerTraining;
            companyDetails.TeamPictureBase64 = details.TeamPictureBase64;
            companyDetails.ContactPersonHRM = details.ContactPersonHRM;
            companyDetails.TrainerProfessionalExperience = details.TrainerProfessionalExperience;
            await unitOfWork.CompanyDetailsRepository.UpdateAsync(companyDetails);
        }

        unitOfWork.Commit();
        await unitOfWork.SaveChanges();
    }

    public async Task<IEnumerable<ActivityResource>> GetCompanyActivitiesAsync(Company company)
    {
        return await unitOfWork.CompanyActivityRepository.GetAllAsync().Result
            .Where(ca => ca.CompanyId == company.Id)
            .Include(a => a.Activity)
            .Select(a => new ActivityResource
            {
                Id = a.Id,
                Name = a.Activity.Name,
                Value = a.Value
            }).ToListAsync();
    }

    public async Task UpdateCompanyActivities(IEnumerable<CompanyActivity> updatedActivities, Company company)
    {
        unitOfWork.BeginTransaction();
        foreach (var updatedActivity in updatedActivities)
        {
            var existingActivity = await unitOfWork.CompanyActivityRepository.GetByIdAsync(updatedActivity.Id);
            if (existingActivity != null)
            {
                existingActivity.Value = updatedActivity.Value;
                existingActivity.Company = company;
                await unitOfWork.CompanyActivityRepository.UpdateAsync(existingActivity);
            }
        }

        await unitOfWork.SaveChanges();
        unitOfWork.Commit();
    }

    public async Task<Company?> AddCompany(int programId, string companyName, User companyUser)
    {
        var program = await unitOfWork.AcademicProgramRepository.GetByIdAsync(programId);
        if (program == null) return null;

        unitOfWork.BeginTransaction();
        var company = new Company
        {
            Name = companyName,
            InstitutionId = program.InstitutionId,
            AcademicProgramId = program.Id,
            User = companyUser
        };
        await unitOfWork.CompanyRepository.AddAsync(company);
        unitOfWork.Commit();
        await unitOfWork.SaveChanges();
        return company;
    }

    public async Task<Company?> GetCompanyByUser(User user)
    {
        var company = await unitOfWork.CompanyRepository.GetAllAsync().Result.Where(c => c.UserId == user.Id)
            .SingleOrDefaultAsync();
        return company;
    }

    public async Task DeleteCompany(int id)
    {
        await unitOfWork.CompanyRepository.DeleteAsync(id);
    }
}