using DualJobDate.BusinessObjects.Entities.Base;
using DualJobDate.BusinessObjects.Entities.Base.Interface;

namespace DualJobDate.BusinessObjects.Entities;

public class Activity : BaseEntity, IBaseAcademicProgramEntity
{
    public string Name { get; set; } = string.Empty;
    public ICollection<Company> Companies { get; set; } = [];
    public ICollection<CompanyActivity> CompanyActivities { get; set; } = [];

    //navigation properties
    public AcademicProgram? AcademicProgram { get; set; }
    public int AcademicProgramId { get; set; }
    public Institution? Institution { get; set; }
    public int InstitutionId { get; set; }
}