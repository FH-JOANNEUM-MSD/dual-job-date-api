using DualJobDate.BusinessObjects.Entities.Models;

namespace DualJobDate.BusinessObjects.Entities.Interface.Service
{
    public interface ICompanyService
    {
        Task<Company?> GetCompanyByIdAsync(int id);

        Task<List<Company>> GetCompaniesByInstitution(int institutionId);
        Task<List<Company>> GetCompaniesByAcademicProgram(int academicProgramId);
        Task UpdateCompany(UpdateCompanyModel companymodel, Company company);
        Task UpdateCompanyActivity(bool isActive, Company company);
        Task UpdateCompanyDetails(CompanyDetails details, Company company);
    }
}
