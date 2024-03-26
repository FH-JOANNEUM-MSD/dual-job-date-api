using System.Collections;
using DualJobDate.BusinessObjects.Dto;
using DualJobDate.BusinessObjects.Entities.Interface.Service;
using System.Text.Json;
using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface;
using DualJobDate.BusinessObjects.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace DualJobDate.BusinessLogic.Services
{
    public class CompanyService(IUnitOfWork unitOfWork) : ICompanyService
    {
        public Task<Company?> GetCompanyByIdAsync(int id)
        {
            var result = unitOfWork.CompanyRepository.GetByIdAsync(id);
            return result;
        }
        
        public Task<List<Company>> GetCompaniesByInstitution(int institutionId)
        {
            var result = unitOfWork.CompanyRepository.GetAllAsync();
            return result.Result.Where(c => c.InstitutionId == institutionId).ToListAsync();
        }
        
        public Task<List<Company>> GetCompaniesByAcademicProgram(int academicProgramId)
        {
            var result = unitOfWork.CompanyRepository.GetAllAsync();
            return result.Result.Where(c => c.AcademicProgramId == academicProgramId).ToListAsync();
        }
        
        public async Task UpdateCompany(UpdateCompanyModel model,Company company)
        {
            unitOfWork.BeginTransaction();
            company.Industry = model.Industry;
            company.LogoBase64 = model.LogoBase64;
            company.Website = model.Website; 
            await unitOfWork.CompanyRepository.UpdateAsync(company);
            unitOfWork.Commit();
            await unitOfWork.SaveChanges();
        }
        public async Task UpdateCompanyActivity(bool isActive,Company company)
        {
            unitOfWork.BeginTransaction();
            company.IsActive = isActive;
            await unitOfWork.CompanyRepository.UpdateAsync(company);
            unitOfWork.Commit();
            await unitOfWork.SaveChanges();
        }
        
        public async Task UpdateCompanyDetails(CompanyDetails details,Company company)
        {
            unitOfWork.BeginTransaction();
            if (company.CompanyDetailsId == null)
            {
                company.CompanyDetails = details;
                await unitOfWork.CompanyDetailsRepository.AddAsync(company.CompanyDetails);
                var result = unitOfWork.CompanyRepository.UpdateAsync(company);
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
    }
}
