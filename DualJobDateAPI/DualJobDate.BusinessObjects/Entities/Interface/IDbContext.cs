using DualJobDate.BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;

namespace DualJobDate.BusinessObjects.Entities.Interface
{
    public interface IDbContext : IDisposable
    {
        
        public DbSet<User> Users { get; set; }

        Task<int> SaveChangesAsync();
    }
}
