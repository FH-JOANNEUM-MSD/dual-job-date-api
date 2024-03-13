using DualJobDate.BusinessObjects.Entities.Base;

namespace DualJobDate.BusinessObjects.Entities
{
    public class Institution : BaseEntity
    {
        public string Name { get; set; }
        public string KeyName { get; set; }
        public string Website { get; set; }
        
        //navigation properties
        public int AddressId { get; set; }
        public Address Address { get; set; }
        public List<User> Users { get; set; } = new();
    }
}
