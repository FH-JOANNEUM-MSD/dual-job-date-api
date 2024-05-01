using DualJobDate.BusinessObjects.Entities.Base;
using DualJobDate.BusinessObjects.Entities.Base.Interface;

namespace DualJobDate.BusinessObjects.Entities;

public class Activity : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public ICollection<Company> Companies { get; set; } = [];
    public ICollection<CompanyActivity> CompanyActivities { get; set; } = [];

    //navigation properties
    public AcademicProgram? AcademicProgram { get; set; }
    public int AcademicProgramId { get; set; }
}