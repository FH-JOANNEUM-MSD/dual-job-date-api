using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DualJobDate.DataAccess;

public class AppDbContext : IdentityDbContext<User, Role, string>, IDbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Institution> Institutions { get; set; }
    public DbSet<AcademicProgram> AcademicPrograms { get; set; }
    public DbSet<Activity> Activities { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<CompanyActivity> CompanyActivities { get; set; }
    public DbSet<CompanyDetails> CompanyDetailsEnumerable { get; set; }
    public DbSet<StudentCompany> StudentCompanies { get; set; }
    public DbSet<UserType> UserTypes { get; set; }

    public Task<int> SaveChangesAsync()
    {
        return base.SaveChangesAsync();
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
            .HasMany(e => e.Activities)
            .WithMany(e => e.Companies)
            .UsingEntity<CompanyActivity>();

        modelBuilder
            .Entity<Role>()
            .Property(x => x.UserTypeEnum)
            .HasConversion<int>();

        modelBuilder.Entity<User>()
            .HasOne(u => u.Company)
            .WithOne(c => c.User)
            .HasForeignKey<Company>(c => c.UserId)
            .IsRequired();

        modelBuilder.Entity<AcademicProgram>(builder =>
        {
            builder.HasIndex(x => new { x.Year, x.KeyName })
                .IsUnique();
        });
    }
}