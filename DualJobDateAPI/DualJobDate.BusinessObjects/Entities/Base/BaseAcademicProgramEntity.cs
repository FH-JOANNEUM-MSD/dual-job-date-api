using DualJobDate.BusinessObjects.Entities.Interface;

namespace DualJobDate.BusinessObjects.Entities.Base
{
    public class BaseAcademicProgramEntity : BaseInstitutionEntity, IBaseAcademicProgramEntity
    {
        public AcademicProgram? AcademicProgram { get; set; }
        public int? AcademicProgramId { get; set; }
    }
}
