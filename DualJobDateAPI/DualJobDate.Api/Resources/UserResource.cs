using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Enum;
using DualJobDate.BusinessObjects.Entities.Interface;

namespace DualJobDate.API.Resources;

public class UserResource
{
    public string Id { get; set; }
    public AcademicProgram? AcademicProgram { get; set; }
    public Institution? Institution { get; set; }
    public bool IsActive { get; set; }
    public UserTypeEnum UserType { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? ProfilePicture { get; set; }
    public DateTime BirthDate { get; set; }
    public bool IsNew { get; set; }
    public string Email { get; set;}
}