namespace DualJobDate.BusinessObjects.Dtos;

public class AddressDto
{
    public string Street { get; set; } = string.Empty;
    public string BuildingNumber { get; set; } = string.Empty;
    public int? ApartmentNumber { get; set; }
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public float? Floor { get; set; }
}