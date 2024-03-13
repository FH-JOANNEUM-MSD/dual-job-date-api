using DualJobDate.BusinessObjects.Entities.Base;

namespace DualJobDate.BusinessObjects.Entities;

public class Address : BaseEntity
{
    public string Street { get; set; }
    public string BuildingNumber { get; set; }
    public int? ApartmentNumber { get; set; }
    public string City { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }
    public float Floor { get; set; }
    
    //navigation properties
    public int? CompanyId { get; set; }
    public Company? Company { get; set; }
    public int? InstitutionId { get; set; }
    public Institution? Institution { get; set; }
}