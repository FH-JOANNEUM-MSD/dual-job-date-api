using DualJobDate.BusinessObjects.Entities.Interface;
using Microsoft.AspNetCore.Identity;

namespace DualJobDate.BusinessObjects.Entities
{
    public class User : IdentityUser<string>, IBaseAcademicProgramEntity
    {
        public AcademicProgram? AcademicProgram { get; set; }
        public int? AcademicProgramId { get; set; }
        public Institution? Institution { get; set; }
        public int InstitutionId { get; set; }
        int IBaseEntity.Id { get; set; }
    }
}
