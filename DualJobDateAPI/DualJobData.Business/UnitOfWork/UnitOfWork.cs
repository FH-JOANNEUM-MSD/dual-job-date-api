using DualJobData.BusinessLogic.Repositories.Interfaces;
using DualJobData.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DualJobData.BusinessLogic.UnitOfWork
{
    public class UnitOfWork(AppDbContext dbContext, IUserRepository userRepository) : IUnitOfWork, IDisposable
    {
        private readonly AppDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        public IUserRepository UserRepository => userRepository ?? throw new ArgumentNullException(nameof(userRepository));
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
