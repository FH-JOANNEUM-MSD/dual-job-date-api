namespace DualJobDate.BusinessObjects.Entities.Models;

public class RegisterCompanyUserFromJsonModel
{
    public required string Email { get; set; }
    public required string CompanyName { get; set; }
    public required int AcademicProgramYear { get; set; }
    public required string AcademicProgramKeyName { get; set; }
    public required string InstitutionKeyName { get; set; }
}