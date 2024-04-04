using DualJobDate.BusinessObjects.Entities.Interface;
using DualJobDate.BusinessObjects.Entities.Interface.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DualJobDate.DataAccess
{
    public class UnitOfWork(AppDbContext dbContext,
        IInstitutionRepository institutionRepository, IAcademicProgramRepository academicProgramRepository, IAcademicDegreeRepository academicDegreeRepository, IActivityRepository activityRepository, ICompanyRepository companyRepository, ICompanyDetailsRepository companyDetailsRepository, ICompanyActivityRepository companyActivityRepository, IAdressRepository adressRepository) : IUnitOfWork, IDisposable
    {
        private readonly AppDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        public IInstitutionRepository InstitutionRepository => institutionRepository ?? throw new ArgumentNullException(nameof(institutionRepository));
        public IAcademicProgramRepository AcademicProgramRepository => academicProgramRepository ?? throw new ArgumentNullException(nameof(academicProgramRepository));
        public IAcademicDegreeRepository AcademicDegreeRepository => academicDegreeRepository ?? throw new ArgumentNullException(nameof(academicDegreeRepository));
        public IActivityRepository ActivityRepository => activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
        public ICompanyRepository CompanyRepository => companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
        public ICompanyDetailsRepository CompanyDetailsRepository => companyDetailsRepository ?? throw new ArgumentNullException(nameof(companyDetailsRepository));
        public ICompanyActivityRepository CompanyActivityRepository => companyActivityRepository ?? throw new ArgumentNullException(nameof(companyActivityRepository));
        public IAdressRepository AdressRepository => adressRepository ?? throw new ArgumentNullException(nameof(adressRepository));
        
        private IDbContextTransaction? _transaction;

        public void BeginTransaction()
        {
            _transaction = _dbContext.Database.BeginTransaction();
        }

        public void Commit()
        {
            try
            {
                _transaction?.Commit();
            }
            catch (InvalidOperationException ex)
            {
                Rollback();
                throw new InvalidOperationException("Error committing transaction.", ex);
            }
            finally
            {
                // Dispose();
            }
        }

        public void Rollback()
        {
            try
            {
                _transaction?.Rollback();
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException("Error rolling back transaction.", ex);
            }
        }

        public async Task SaveChanges()
        {
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Error updating entities.", ex);
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _dbContext.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
