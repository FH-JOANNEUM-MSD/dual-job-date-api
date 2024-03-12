namespace DualJobDate.BusinessObjects.Entities;

public class Address
{
    public string Street { get; set; }
    public string BuildingNumber { get; set; }
    public int? ApartmentNumber { get; set; }
    public string City { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }
    public float Floor { get; set; }
}