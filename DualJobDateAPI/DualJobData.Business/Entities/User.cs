using DualJobData.BusinessLogic.Entities.Base;
using Microsoft.AspNetCore.Identity;

namespace DualJobData.BusinessLogic.Entities
{
    public class User : IdentityUser<string>, IBaseStationEntity
    {
        public Station? Station { get; set; }
        public int? StationId { get; set; }
        public Tenant? Tenant { get; set; }
        public int TenantId { get; set; }
        int IBaseEntity.Id { get; set; }
    }
}
