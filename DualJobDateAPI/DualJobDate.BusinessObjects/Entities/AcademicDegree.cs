using DualJobDate.BusinessObjects.Entities.Base;
using DualJobDate.BusinessObjects.Entities.Enum;

namespace DualJobDate.BusinessObjects.Entities
{
    public class AcademicDegree : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public AcademicDegreeEnum AcademicDegreeEnum { get; set; }
    } 
}