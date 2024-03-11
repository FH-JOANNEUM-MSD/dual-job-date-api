using DualJobDate.BusinessObjects.Entities;

namespace DualJobDate.BusinessObjects.Entities.Interface
{
    public interface IBaseAcademicProgramEntity : IBaseInstitutionEntity
    {
        public AcademicProgram? AcademicProgram { get; set; }
        public int? AcademicProgramId { get; set; }
    }
}
