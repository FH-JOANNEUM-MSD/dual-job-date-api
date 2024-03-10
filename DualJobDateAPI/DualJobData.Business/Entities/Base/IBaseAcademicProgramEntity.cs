namespace DualJobData.BusinessLogic.Entities.Base
{
    public interface IBaseAcademicProgramEntity : IBaseInstitutionEntity
    {
        public AcademicProgram? AcademicProgram { get; set; }
        public int? AcademicProgramId { get; set; }
    }
}
