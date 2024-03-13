using DualJobDate.BusinessObjects.Entities.Base;

namespace DualJobDate.BusinessObjects.Entities;

public class Activity : BaseEntity
{
    public string Name { get; set; }
    
    //navigation properties
    public int AcademicProgramId { get; set; }
    public AcademicProgram AcademicProgram { get; set; }
    public List<CompanyActivity> CompanyActivities { get; set; } = new();
}