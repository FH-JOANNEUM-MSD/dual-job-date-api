using DualJobDate.BusinessObjects.Entities.Base;
using DualJobDate.BusinessObjects.Entities.Enums;

namespace DualJobDate.BusinessObjects.Entities
{
    public class AcademicDegree : BaseEntity
    {
        public string Name { get; set; }
        public AcademicDegreeEnum AcademicDegreeEnum { get; set; }
    } 
}