using DualJobDate.BusinessObjects.Entities.Base;

namespace DualJobDate.BusinessObjects.Entities
{
    public class Match : BaseEntity
    {
        public int StudentId { get; set; }
        public bool? StudentMatch { get; set; }
        public int CompanyId { get; set; }
        public bool? CompanyMatch { get; set; }
    }

}