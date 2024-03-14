using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface;
using Microsoft.EntityFrameworkCore;

namespace DualJobDate.DataAccess
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IDbContext
    {
        public DbSet<Institution> Institutions { get; set; }
        public DbSet<AcademicProgram> AcademicPrograms { get; set; }
        public Task<int> SaveChangesAsync()
        {
            return base.SaveChangesAsync();
        }

        void IDisposable.Dispose() => base.Dispose();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<UserType>()
                .Property(x => x.UserTypeEnum)
                .HasConversion<int>();
            
            modelBuilder
                .Entity<AcademicDegree>()
                .Property(x => x.AcademicDegreeEnum)
                .HasConversion<int>();
        }
    }
}
