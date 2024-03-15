using DualJobDate.BusinessObjects.Entities;

namespace DualJobDate.BusinessObjects.Entities.Interface
{
    public interface IBaseInstitutionEntity : IBaseEntity
    {
        public Institution? Institution { get; set; }
        public int? InstitutionId { get; set; }
    }
}
