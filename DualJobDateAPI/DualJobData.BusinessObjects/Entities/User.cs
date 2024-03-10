using DualJobData.BusinessLogic.Entities.Base;
using Microsoft.AspNetCore.Identity;

namespace DualJobData.BusinessLogic.Entities
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
