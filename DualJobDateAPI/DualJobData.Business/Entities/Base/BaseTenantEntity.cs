namespace DualJobData.BusinessLogic.Entities.Base
{
    public class BaseTenantEntity : BaseEntity, IBaseTenantEntity
    {
        public Tenant? Tenant { get; set; }
        public int TenantId { get; set; }
    }
}
