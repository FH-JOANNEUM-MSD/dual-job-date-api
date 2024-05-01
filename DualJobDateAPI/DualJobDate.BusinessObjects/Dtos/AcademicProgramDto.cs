using DualJobDate.BusinessObjects.Entities.Enum;

namespace DualJobDate.BusinessObjects.Dtos;

public class AcademicProgramDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string KeyName { get; set; } = string.Empty;
    public int InstitutionId { get; set; }
    public AcademicDegreeEnum AcademicDegreeEnum { get; set; } = AcademicDegreeEnum.Default;
}