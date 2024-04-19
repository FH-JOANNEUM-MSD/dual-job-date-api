namespace DualJobDate.BusinessObjects.Entities.Models;

public class UpdateCompanyModel
{
    public string? Name { get; set; }
    public string? Industry { get; set; }
    public string? LogoBase64 { get; set; }
    public string? Website { get; set; }
    
    public string? ShortDescription { get; set; }
    public string? TeamPictureBase64 { get; set; }
    public string? JobDescription { get; set; }
    public string? ContactPersonInCompany { get; set; }
    public string? ContactPersonHRM { get; set; }
    public string? Trainer { get; set; }
    public string? TrainerTraining { get; set; }
    public string? TrainerProfessionalExperience { get; set; }
    public string? TrainerPosition { get; set; }
}