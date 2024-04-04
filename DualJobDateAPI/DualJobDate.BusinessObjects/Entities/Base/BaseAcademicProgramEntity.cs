using DualJobDate.BusinessObjects.Entities.Base.Interface;

namespace DualJobDate.BusinessObjects.Entities.Base
{
    public class BaseAcademicProgramEntity : BaseInstitutionEntity, IBaseAcademicProgramEntity
    {
        public AcademicProgram? AcademicProgram { get; set; }
        public int AcademicProgramId { get; set; }
    }
}
