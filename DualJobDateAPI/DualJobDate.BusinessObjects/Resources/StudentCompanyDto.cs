namespace DualJobDate.BusinessObjects.Resources;

public class StudentCompanyDto
{
    public string StudentId { get; set; } = String.Empty;
    public int CompanyId { get; set; }
    public bool Like { get; set; }
}