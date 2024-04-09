using DualJobDate.BusinessObjects.Entities.Base.Interface;

namespace DualJobDate.BusinessObjects.Entities.Base;

public abstract class BaseEntity : IBaseEntity
{
    public int Id { get; set; }
}