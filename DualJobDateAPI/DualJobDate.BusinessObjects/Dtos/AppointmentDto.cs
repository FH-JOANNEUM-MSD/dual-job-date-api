using DualJobDate.BusinessObjects.Entities;

namespace DualJobDate.BusinessObjects.Dtos;

public class AppointmentDto
{
    public int Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string UserId { get; set; }
    public UserDto User { get; set; }
    public string CompanyId { get; set; }
    public CompanyDto Company { get; set; }
}