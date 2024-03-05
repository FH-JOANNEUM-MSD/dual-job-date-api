namespace DualJobData.BusinessLogic.Entities
{
    public class BaseStationEntity : BaseTenantEntity
    {
        public Station? Station { get; set; }
        public int? StationId { get; set; }
    }
}
