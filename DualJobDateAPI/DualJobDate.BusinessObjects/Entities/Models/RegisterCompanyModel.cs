namespace DualJobDate.BusinessObjects.Entities.Models;

public class RegisterCompanyModel
{
    public required int AcademicProgramId { get; set; }
    public required string CompanyName { get; set; }
    public required string UserEmail { get; set; }
}