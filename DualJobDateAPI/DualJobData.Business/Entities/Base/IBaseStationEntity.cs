namespace DualJobData.BusinessLogic.Entities.Base
{
    public interface IBaseStationEntity : IBaseTenantEntity
    {
        public Station? Station { get; set; }
        public int? StationId { get; set; }
    }
}
