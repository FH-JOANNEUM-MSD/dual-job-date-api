using DualJobDate.BusinessObjects.Entities.Base;

namespace DualJobDate.BusinessObjects.Entities
{
    public class Institution : BaseEntity
    {
        public string Name { get; set; }
        public string KeyName { get; set; }
        public string? Website { get; set; }
        
        //navigation properties
        public List<Address> Addresses { get; set; } = new();
        public List<ApplicationUser> Users { get; set; } = new();
    }
}
