using DualJobDate.BusinessObjects.Entities.Interface;
using DualJobDate.BusinessObjects.Entities.Interface.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DualJobDate.DataAccess
{
    public class UnitOfWork(AppDbContext dbContext, IUserRepository userRepository,
        IInstitutionRepository institutionRepository) : IUnitOfWork, IDisposable
    {
        private readonly AppDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        public IUserRepository UserRepository => userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        public IInstitutionRepository InstitutionRepository => institutionRepository ?? throw new ArgumentNullException(nameof(institutionRepository));

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
                Dispose();
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
