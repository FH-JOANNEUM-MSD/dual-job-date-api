using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Models;

namespace DualJobDate.BusinessObjects.Dtos;

public class CompanyDto
{
    //CompanyID
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Industry { get; set; }
    public string? LogoBase64 { get; set; }
    public string? Website { get; set; }
    public bool IsActive { get; set; }
    public AcademicProgramDto AcademicProgram{ get; set; }
    public CompanyUserDto User { get; set; }
    public InstitutionDto Institution { get; set; }
    public CompanyDetailsDto CompanyDetails { get; set; }
    public List<ActivityDto> Activities { get; set; }
    public List<AddressDto> Addresses { get; set; }
}