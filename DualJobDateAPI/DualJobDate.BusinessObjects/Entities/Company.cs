using DualJobDate.BusinessObjects.Entities.Base;

namespace DualJobDate.BusinessObjects.Entities
{
    public class Company: BaseEntity
    {
        public string Name { get; set; }
        public string? Industry { get; set; }
        public string? LogoBase64 { get; set; }
        public string? Website { get; set; }
        public bool isActive { get; set; }
    
    
        //navigation properties
        public ApplicationUser? User { get; set; }
        public int CompanyDetailsId { get; set; }
        public CompanyDetails? CompanyDetails { get; set; }
        public List<Address> Addresses { get; set; } = new();
        public List<CompanyActivity> CompanyActivities { get; set; } = new();
        public List<AcademicProgram> AcademicPrograms { get; set; } = new();
    }
}