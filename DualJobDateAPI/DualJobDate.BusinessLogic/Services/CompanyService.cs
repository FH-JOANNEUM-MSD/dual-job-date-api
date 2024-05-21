using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface;
using DualJobDate.BusinessObjects.Entities.Interface.Service;
using DualJobDate.BusinessObjects.Entities.Models;
using DualJobDate.BusinessObjects.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DualJobDate.BusinessLogic.Services;

public class CompanyService(IUnitOfWork unitOfWork, UserManager<User> userManager) : ICompanyService
{
    public Task<Company?> GetCompanyByIdAsync(int id)
    {
        var result = unitOfWork.CompanyRepository.GetAllAsync().Result.
            Include(x => x.AcademicProgram).
            Include(x => x.Institution).
            Include(x => x.User).
            Include(x => x.CompanyDetails).
            Include(x => x.Activities).
            Include(x => x.Addresses).
            Where(x => x.Id == id).SingleOrDefaultAsync();
        return result;
    }

    public async Task<List<Company>> GetActiveCompaniesAsync(User user)
    {
        // Fetch all active companies related to the specified academicProgramId
        // and include the StudentCompany data specifically for the given user.
        var result = await unitOfWork.CompanyRepository
            .GetAllAsync().Result
            .Include(c => c.StudentCompanies.Where(sc => sc.StudentId == user.Id)). // Include only StudentCompany for the given user
            Include(x => x.CompanyDetails).
            Include(x => x.Activities)
            .Include(x => x.CompanyActivities).
            Include(x => x.Addresses)
            .Where(c => c.AcademicProgramId == user.AcademicProgramId && c.IsActive)
            .ToListAsync();
        return result;
    }



    public async Task<List<Company>> GetCompaniesByInstitutionAsync(int institutionId)
    {
        var companies = unitOfWork.CompanyRepository.GetAllAsync();
        return companies.Result.Where(c => c.InstitutionId == institutionId).ToList();
    }

    public async Task<List<Company>> GetCompaniesByAcademicProgramAsync(int academicProgramId)
    {
        var companies = await unitOfWork.CompanyRepository.GetAllAsync();
        return companies.Where(c => c.AcademicProgramId == academicProgramId).ToList();
    }

    public async Task UpdateCompany(UpdateCompanyModel model, Company company)
    {
        unitOfWork.BeginTransaction();
        
        if (string.IsNullOrWhiteSpace(model.Name))
        {
            throw new ArgumentException("Company Name cannot be null or whitespace.");
        }

        if (string.IsNullOrWhiteSpace(model.Industry))
        {
            throw new ArgumentException("Industry cannot be null or whitespace.");
        }
    
        if (!string.IsNullOrWhiteSpace(model.Website) && !Uri.IsWellFormedUriString(model.Website, UriKind.Absolute))
        {
            throw new ArgumentException("Website must be a valid URL.");
        }

        if (!string.IsNullOrWhiteSpace(model.LogoBase64) && !IsBase64String(model.LogoBase64))
        {
            throw new ArgumentException("LogoBase64 must be a valid Base64 string.");
        }

        company.Industry = model.Industry;
        company.LogoBase64 = model.LogoBase64;
        company.Website = model.Website;

        await unitOfWork.CompanyRepository.UpdateAsync(company);

        unitOfWork.Commit();
        await unitOfWork.SaveChanges();
    }

    private bool IsBase64String(string base64)
    {
        Span<byte> buffer = new Span<byte>(new byte[base64.Length]);
        return Convert.TryFromBase64String(base64, buffer, out int bytesParsed);
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
        return await unitOfWork.CompanyDetailsRepository.GetByIdAsync(company.CompanyDetailsId.Value);
    }

    public async Task UpdateCompanyDetails(CompanyDetails details, Company company)
    {
        unitOfWork.BeginTransaction();
        
        if (details == null)
        {
            throw new ArgumentNullException(nameof(details));
        }

        if (string.IsNullOrWhiteSpace(details.Trainer))
        {
            throw new ArgumentException("Trainer cannot be null or whitespace.");
        }
        
        if (string.IsNullOrWhiteSpace(details.ContactPersonInCompany))
        {
             throw new ArgumentException("Contact person in company cannot be null or whitespace.");
        }
        
        if (!string.IsNullOrWhiteSpace(details.TeamPictureBase64) && !IsBase64String(details.TeamPictureBase64))
        {
            throw new ArgumentException("TeamPictureBase64 must be a valid Base64 string.");
        }
        
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

    public async Task<IEnumerable<ActivityDto>> GetCompanyActivitiesAsync(Company company)
    {
        return await unitOfWork.CompanyActivityRepository.GetAllAsync().Result
            .Where(ca => ca.CompanyId == company.Id)
            .Include(a => a.Activity)
            .Select(a => new ActivityDto
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
        unitOfWork.Commit();
        await unitOfWork.SaveChanges();
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

    public async Task AddLocations(IEnumerable<Address> addresses, Company company)
    {
        unitOfWork.BeginTransaction();
        foreach (var address in addresses)
        {
            address.Company = company;
            await unitOfWork.AddressRepository.AddAsync(address);
        }
        unitOfWork.Commit();
    }

    public async Task<IEnumerable<Address>> GetLocationsByCompanyAsync(Company company)
    {
        return await unitOfWork.AddressRepository.GetAllAsync().Result.Where(a => a.CompanyId == company.Id).ToListAsync();
    }
}