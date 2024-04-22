using DualJobDate.BusinessObjects.Entities.Enum;

namespace DualJobDate.BusinessObjects.Dtos;

public class UserDto
{
    public string Id { get; set; }
    public InstitutionDto Institution { get; set; }
    public AcademicProgramDto AcademicProgram { get; set; }
    public UserTypeEnum UserType { get; set; }
    public bool IsNew { get; set; }
    public string Email { get; set; }
    public UserCompanyDto Company { get; set; }
}