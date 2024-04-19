using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Models;

namespace DualJobDate.BusinessObjects.Resources;

public class CompanyResource
{
    //CompanyID
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Industry { get; set; }
    public string? LogoBase64 { get; set; }
    public string? Website { get; set; }
    public bool IsActive { get; set; }
    public AcademicProgramResource AcademicProgram{ get; set; }
    public CompanyUserResource User { get; set; }
    public InstitutionResource Institution { get; set; }
    public CompanyDetailsResource CompanyDetails { get; set; }
    public List<ActivityResource> Activities { get; set; }
    public List<AdressResource> Addresses { get; set; }
}