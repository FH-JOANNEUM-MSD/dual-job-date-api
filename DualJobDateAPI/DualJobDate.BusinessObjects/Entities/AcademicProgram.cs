using DualJobDate.BusinessObjects.Entities.Base;
using DualJobDate.BusinessObjects.Entities.Base.Interface;
using DualJobDate.BusinessObjects.Entities.Enum;

namespace DualJobDate.BusinessObjects.Entities
{
    public class AcademicProgram : BaseEntity, IBaseInstitutionEntity
    {
        public int Year { get; set; }
        public string Name { get; set; } = string.Empty;
        public string KeyName { get; set; } = string.Empty;
        public AcademicDegreeEnum AcademicDegreeEnum { get; set; } = AcademicDegreeEnum.Default;
        
        //navigation properties
        public int InstitutionId { get; set; }
        public Institution? Institution { get; set; }
        public ICollection<User> Users { get; set; } = [];
        public ICollection<Activity> Activities { get; set; } = [];
        public ICollection<Company> Companies { get; set; } = [];
    }
}
