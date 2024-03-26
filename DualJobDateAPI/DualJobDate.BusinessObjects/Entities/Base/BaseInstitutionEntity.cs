using DualJobDate.BusinessObjects.Entities.Base.Interface;

namespace DualJobDate.BusinessObjects.Entities.Base
{
    public class BaseInstitutionEntity : BaseEntity, IBaseInstitutionEntity
    {
        public Institution? Institution { get; set; }
        public int InstitutionId { get; set; }
    }
}
