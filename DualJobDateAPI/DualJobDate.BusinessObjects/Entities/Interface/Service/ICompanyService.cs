using DualJobDate.BusinessObjects.Entities.Models;
using DualJobDate.BusinessObjects.Dtos;

namespace DualJobDate.BusinessObjects.Entities.Interface.Service;

public interface ICompanyService
{
    Task<Company?> GetCompanyByIdAsync(int id);
    Task<List<Company>> GetActiveCompaniesAsync(User user);
    Task<List<Company>> GetCompaniesByInstitutionAsync(int institutionId);
    Task<List<Company>> GetCompaniesByAcademicProgramAsync(int academicProgramId);
    Task UpdateCompany(UpdateCompanyModel companymodel, Company company);
    Task UpdateCompanyActivity(bool isActive, Company company);
    Task UpdateCompanyDetails(CompanyDetails details, Company company);
    Task<CompanyDetails?> GetCompanyDetailsAsync(Company company);
    Task<IEnumerable<ActivityDto>> GetCompanyActivitiesAsync(Company company);
    Task UpdateCompanyActivities(IEnumerable<CompanyActivity> activities, Company company);
    Task<Company?> AddCompany(int programId, string companyName, User companyUser);
    Task<Company?> GetCompanyByUser(User user);
    Task DeleteCompany(int id);
    // Task AddLocations(IEnumerable<Address> addresses, Company company);
    // Task<IEnumerable<Address>> GetLocationsByCompanyAsync(Company company);
}