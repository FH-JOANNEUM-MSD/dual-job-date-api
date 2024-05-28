using DualJobDate.BusinessObjects.Entities;

namespace DualJobDate.BusinessObjects.Dtos;

public class AppointmentDto
{
    public DateTime AppointmentDate { get; set; }
    public String UserId { get; set; }
    public CompanyDto Company { get; set; }
}