namespace DualJobData.BusinessLogic.Entities.Base
{
    public class BaseStationEntity : BaseTenantEntity, IBaseStationEntity
    {
        public Station? Station { get; set; }
        public int? StationId { get; set; }
    }
}
