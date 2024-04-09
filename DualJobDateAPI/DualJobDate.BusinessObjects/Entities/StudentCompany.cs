using DualJobDate.BusinessObjects.Entities.Base;

namespace DualJobDate.BusinessObjects.Entities;

public class StudentCompany : BaseEntity
{
    public string StudentId { get; set; }
    public User? Student { get; set; }
    public int CompanyId { get; set; }
    public Company? Company { get; set; }

    public bool Like { get; set; } // 0 = Dislike, 1 = Like, Not in List = Neutral
}