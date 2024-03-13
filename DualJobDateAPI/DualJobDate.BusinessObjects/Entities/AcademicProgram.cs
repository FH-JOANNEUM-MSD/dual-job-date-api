using DualJobDate.BusinessObjects.Entities.Base;
using DualJobDate.BusinessObjects.Entities.Enums;
using DualJobDate.BusinessObjects.Entities.Interface;

namespace DualJobDate.BusinessObjects.Entities
{
    public class AcademicProgram : BaseEntity, IBaseInstitutionEntity
    {
        public int Year { get; set; }
        public string Name { get; set; }
        public string KeyName { get; set; }
        public AcademicDegreeEnum AcademicDegreeEnum { get; set; } = AcademicDegreeEnum.Default;
        
        //navigation properties
        public List<User> Users { get; set; } = new();
        public int InstitutionId { get; set; }
        public Institution Institution { get; set; }
        
    }
}
