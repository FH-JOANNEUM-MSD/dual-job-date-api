using DualJobDate.BusinessObjects.Entities.Base;

namespace DualJobDate.BusinessObjects.Entities;

public class Appointment : BaseEntity
{
    public DateTime AppointmentDate { get; set; }
    public String UserId { get; set; }
    public int CompanyId { get; set; }
    
    // Navigation properties
    public User User { get; set; }
    public Company Company { get; set; }
}