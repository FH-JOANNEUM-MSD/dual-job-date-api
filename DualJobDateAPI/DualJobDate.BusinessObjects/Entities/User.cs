using DualJobDate.BusinessObjects.Entities.Enum;
using Microsoft.AspNetCore.Identity;

namespace DualJobDate.BusinessObjects.Entities;

public class User : IdentityUser
{
    public bool IsActive { get; set; }
    public UserTypeEnum UserType { get; set; }
    public bool IsNew { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    //navigation properties
    public int AcademicProgramId { get; set; }
    public AcademicProgram? AcademicProgram { get; set; }
    public int InstitutionId { get; set; }
    public Institution? Institution { get; set; }
    public Company? Company { get; set; }
    public ICollection<Company> Likes { get; set; } = [];
    public ICollection<StudentCompany> StudentCompanies { get; set; } = [];
    public ICollection<Appointment> Appointments { get; set; } = [];
}