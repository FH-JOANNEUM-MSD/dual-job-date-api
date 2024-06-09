using DualJobDate.BusinessObjects.Entities.Enum;

namespace DualJobDate.BusinessObjects.Entities.Models;

public class AcademicProgramModel
{
    public required int Year { get; set; }
    public required string Name { get; set; }
    public required string KeyName { get; set; }
    public AcademicDegreeEnum AcademicDegreeEnum { get; set; } = AcademicDegreeEnum.Default;
}