using DualJobDate.BusinessObjects.Entities.Enum;
using DualJobDate.BusinessObjects.Entities.Interface;
using Microsoft.AspNetCore.Identity;

namespace DualJobDate.BusinessObjects.Entities
{
    public class User : IdentityUser, IBaseAcademicProgramEntity
    {
        public AcademicProgram? AcademicProgram { get; set; }
        public int? AcademicProgramId { get; set; }
        public Institution? Institution { get; set; }
        public int? InstitutionId { get; set; }
        int IBaseEntity.Id { get; set; }
        public bool IsActive { get; init; } = true;
        public UserTypeEnum UserType { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? ProfilePicture { get; set; }
        public DateTime BirthDate { get; set; }
        public bool IsNew { get; init; } = true;
        // Mail in IdentityUser
    }
}
