namespace DualJobData.BusinessLogic.Entities
{
    public class BaseTenantEntity : BaseEntity
    {
        public Tenant? Tenant { get; set; }
        public int TenantId { get; set; }
    }
}
