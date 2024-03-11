using DualJobDate.BusinessObjects.Entities.Interface;

namespace DualJobDate.BusinessObjects.Entities.Base
{
    public abstract class BaseEntity : IBaseEntity
    {
        public int Id { get; set; }
    }
}
