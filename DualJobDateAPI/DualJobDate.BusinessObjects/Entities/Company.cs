using DualJobDate.BusinessObjects.Entities.Base;
using DualJobDate.BusinessObjects.Entities.Base.Interface;

namespace DualJobDate.BusinessObjects.Entities;

public class Company : BaseEntity, IBaseAcademicProgramEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Industry { get; set; }
    public string? LogoBase64 { get; set; }
    public string? Website { get; set; }
    public bool IsActive { get; set; }


    //navigation properties
    public string UserId { get; set; }
    public User User { get; set; }
    public int? CompanyDetailsId { get; set; }
    public CompanyDetails? CompanyDetails { get; set; }
    public ICollection<Address> Addresses { get; set; } = [];
    public ICollection<Activity> Activities { get; set; } = [];
    public ICollection<CompanyActivity> CompanyActivities { get; set; } = [];
    public ICollection<User> Likers { get; set; } = [];
    public ICollection<StudentCompany> StudentCompanies { get; set; } = [];
    public AcademicProgram? AcademicProgram { get; set; }
    public int AcademicProgramId { get; set; }
    public Institution? Institution { get; set; }
    public int InstitutionId { get; set; }
}