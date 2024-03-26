using System.Security.Claims;
using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DualJobDate.DataAccess
{
    public class AppDbContext: IdentityDbContext<User, IdentityRole, string>, IDbContext
    {
        public DbSet<Institution> Institutions { get; set; }
        public DbSet<AcademicProgram> AcademicPrograms { get; set; }
        public DbSet<AcademicDegree> AcademicDegrees { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<CompanyActivity> CompanyActivities { get; set; }
        public DbSet<CompanyDetails> CompanyDetails { get; set; }
        public DbSet<StudentCompany> StudentCompanies { get; set; }
        public DbSet<UserType> UserTypes { get; set; }
        
        public Task<int> SaveChangesAsync()
        {
            return base.SaveChangesAsync();
        }
        
        public AppDbContext()
        {
        }
    
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<IdentityUserLogin<string>>()
                .HasKey(l => l.UserId);

            modelBuilder.Entity<IdentityUserRole<string>>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<IdentityUserToken<string>>()
                .HasKey(t => new { t.UserId, t.LoginProvider, t.Name });

            modelBuilder.Entity<User>()
                .HasMany(e => e.Likes)
                .WithMany(e => e.Likers)
                .UsingEntity<StudentCompany>();
            
            modelBuilder.Entity<Company>()
                .HasOne(c => c.User)
                .WithOne(u => u.Company)
                .HasForeignKey<User>(u => u.CompanyId);
                
            modelBuilder.Entity<Company>()
                .HasMany(e => e.Activities)
                .WithMany(e => e.Companies)
                .UsingEntity<CompanyActivity>();
            
            modelBuilder
                .Entity<AcademicDegree>()
                .Property(x => x.AcademicDegreeEnum)
                .HasConversion<int>();

            modelBuilder.Entity<AcademicProgram>(builder =>
            {
                builder.HasIndex(x => new { x.Year, x.KeyName })
                    .IsUnique();
            });
        }
    }
}
