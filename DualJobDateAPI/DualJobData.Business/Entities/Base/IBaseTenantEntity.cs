namespace DualJobData.BusinessLogic.Entities.Base
{
    public interface IBaseTenantEntity : IBaseEntity
    {
        public Tenant? Tenant { get; set; }
        public int TenantId { get; set; }
    }
}
