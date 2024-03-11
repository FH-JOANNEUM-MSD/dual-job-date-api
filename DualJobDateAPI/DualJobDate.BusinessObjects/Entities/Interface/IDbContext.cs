using DualJobDate.BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;

namespace DualJobDate.BusinessObjects.Entities.Interface
{
    public interface IDbContext : IDisposable
    {
        DbSet<Institution> Institutions { get; set; }
        DbSet<AcademicProgram> AcademicPrograms { get; set; }
        Task<int> SaveChangesAsync();
    }
}
