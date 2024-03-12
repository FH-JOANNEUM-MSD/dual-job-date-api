using DualJobDate.BusinessObjects.Entities.Base;

namespace DualJobDate.BusinessObjects.Entities
{
    public class Institution : BaseEntity
    {
        public string Name { get; set; }
        public string KeyName { get; set; }
        public string Website { get; set; }
        public Address Address { get; set; }
    }
}
