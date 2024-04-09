namespace DualJobDate.BusinessObjects.Entities.Interface.Repository;

public interface IBaseRepository<T> : IDisposable
{
    Task<T?> GetByIdAsync(int id);
    Task<IQueryable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
    Task SaveAsync();
}