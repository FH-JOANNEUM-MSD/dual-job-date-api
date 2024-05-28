using DualJobDate.BusinessObjects.Entities;

namespace DualJobDate.BusinessObjects.Dtos;

public class AppointmentDto
{
    public int Id { get; set; }
    public DateTime AppointmentDate { get; set; }
    public String UserId { get; set; }
    public String CompanyId { get; set; }
    public string Company { get; set; }
}