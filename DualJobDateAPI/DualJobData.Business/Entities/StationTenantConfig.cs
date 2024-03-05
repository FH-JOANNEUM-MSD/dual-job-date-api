namespace DualJobData.BusinessLogic.Entities
{
    public class StationTenantConfig
    {
        public StationTenantConfig(int tenantId, int? stationId)
        {
            TenantId = tenantId;
            StationId = stationId;
        }

        public int? StationId { get; set; }
        public int TenantId { get; set; }
    }
}
