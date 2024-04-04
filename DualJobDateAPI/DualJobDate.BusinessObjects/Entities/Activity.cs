using DualJobDate.BusinessObjects.Entities.Base;

namespace DualJobDate.BusinessObjects.Entities
{
    public class Activity : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
    
        //navigation properties
        public int AcademicProgramId { get; set; }
        public AcademicProgram? AcademicProgram { get; set; }
        public ICollection<Company> Companies { get; set; } = [];
        public ICollection<CompanyActivity> CompanyActivities { get; set; } = [];
    }
}