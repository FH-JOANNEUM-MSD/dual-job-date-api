using DualJobDate.BusinessObjects.Entities.Base;
using DualJobDate.BusinessObjects.Entities.Enums;

namespace DualJobDate.BusinessObjects.Entities
{
    public class AcademicProgram : BaseInstitutionEntity
    {
        public int Year { get; set; }
        public string Name { get; set; }
        public string KeyName { get; set; }
        public AcademicDegreeEnum AcademicDegreeEnum { get; set; } = AcademicDegreeEnum.Default;
    }
}
