using DualJobDate.BusinessObjects.Entities.Enum;

namespace DualJobDate.BusinessObjects.Dtos;

public class AcademicProgramDto
{
    public string Id { get; set; }
    public int Year { get; set; }
    public string Name { get; set; } = string.Empty;
    public string KeyName { get; set; } = string.Empty;
    public AcademicDegreeEnum AcademicDegreeEnum { get; set; } = AcademicDegreeEnum.Default;
    public ICollection<UserDto> Users { get; set; } = [];
    public ICollection<ActivityDto> Activities { get; set; } = [];
    public ICollection<CompanyDto> Companies { get; set; } = [];
}