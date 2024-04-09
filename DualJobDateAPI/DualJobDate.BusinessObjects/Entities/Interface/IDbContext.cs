namespace DualJobDate.BusinessObjects.Entities.Interface;

public interface IDbContext : IDisposable
{
    Task<int> SaveChangesAsync();
}