namespace DualJobData.BusinessLogic.Entities.Base
{
    public class BaseAcademicProgramEntity : BaseInstitutionEntity, IBaseAcademicProgramEntity
    {
        public AcademicProgram? AcademicProgram { get; set; }
        public int? AcademicProgramId { get; set; }
    }
}
