namespace DualJobDate.BusinessObjects.Entities.Base.Interface
{
    public interface IBaseAcademicProgramEntity : IBaseInstitutionEntity
    {
        public AcademicProgram? AcademicProgram { get; set; }
        public int AcademicProgramId { get; set; }
    }
}
