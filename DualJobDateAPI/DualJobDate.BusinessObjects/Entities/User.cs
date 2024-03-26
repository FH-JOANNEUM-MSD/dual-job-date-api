using DualJobDate.BusinessObjects.Entities.Base.Interface;
using DualJobDate.BusinessObjects.Entities.Enum;
using Microsoft.AspNetCore.Identity;

namespace DualJobDate.BusinessObjects.Entities
{
    public class User : IdentityUser
    {
        public bool IsActive { get; set; }
        public UserTypeEnum UserType { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? ProfilePicture { get; set; }
        public DateTime BirthDate { get; set; }
        public bool IsNew { get; set; }
        public List<Match> Matches { get; set; }
        // Mail in IdentityUser
        
        //navigation properties
        public int? AcademicProgramId { get; set; }
        public AcademicProgram? AcademicProgram { get; set; }
        public int? InstitutionId { get; set; }
        public Institution? Institution { get; set; }
        public int? CompanyId { get; set; }
        public Company? Company { get; set; }
    }
}
