using DualJobDate.BusinessObjects.Entities.Base;

namespace DualJobDate.BusinessObjects.Entities;

public class Company: BaseEntity
{
    public string Name { get; set; }
    public string Industry { get; set; }
    public string LogoBase64 { get; set; }
    public string Website { get; set; }
    public bool isActive { get; set; }
    
    
    //navigation properties
    public int UserId { get; set; }
    public User User { get; set; }
    public int CompanyDetailsId { get; set; }
    public CompanyDetails CompanyDetails { get; set; }
    public List<Address> Addresses { get; set; } = new();
}