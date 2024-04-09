namespace DualJobDate.BusinessObjects.Resources;

public class CompanyResource
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Industry { get; set; }
    public string? LogoBase64 { get; set; }
    public string? Website { get; set; }
    public bool IsActive { get; set; }
    public int AcademicProgramId { get; set; }
    public int InstitutionId { get; set; }
    public string UserId { get; set; }
}