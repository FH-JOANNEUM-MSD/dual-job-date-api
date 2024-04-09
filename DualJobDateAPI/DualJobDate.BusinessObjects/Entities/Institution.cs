using DualJobDate.BusinessObjects.Entities.Base;

namespace DualJobDate.BusinessObjects.Entities;

public class Institution : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string KeyName { get; set; } = string.Empty;
    public string? Website { get; set; }

    //navigation properties
    public ICollection<Address> Addresses { get; set; } = [];
    public ICollection<User> Users { get; set; } = [];
}