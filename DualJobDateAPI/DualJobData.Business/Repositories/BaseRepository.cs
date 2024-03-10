using DualJobData.BusinessLogic.Entities.Base;
using DualJobData.BusinessLogic.Repositories.Interfaces;
using DualJobData.DataAccess;

namespace DualJobData.BusinessLogic.Repositories
{
    public class BaseRepository<T>(AppDbContext dbContext) : IBaseRepository<T>
        where T : class, IBaseEntity
    {
        private readonly AppDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

        public async Task<T?> GetByIdAsync(int id)
        {
            var entity = await _dbContext.Set<T>().FindAsync(id);
            return entity;
        }

        public async Task<IQueryable<T>> GetAllAsync()
        {
            return await Task.FromResult(_dbContext.Set<T>().AsQueryable());
        }

        public async Task AddAsync(T entity)
        {
            _dbContext.Set<T>().Add(entity);
            await SaveAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _dbContext.Set<T>().Update(entity);
            await SaveAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity is null)
            {
                return;
            }
            _dbContext.Set<T>().Remove(entity);
            await SaveAsync();
        }

        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
