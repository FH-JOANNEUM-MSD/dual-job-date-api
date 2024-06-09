using DualJobDate.BusinessObjects.Entities.Base;

namespace DualJobDate.BusinessObjects.Entities;

public class CompanyDetails : BaseEntity
{
    public string? ShortDescription { get; set; }
    public string? TeamPictureBase64 { get; set; }
    public string? JobDescription { get; set; }
    public string? ContactPersonInCompany { get; set; }
    public string? ContactPersonHRM { get; set; }
    public string? Trainer { get; set; }
    public string? TrainerTraining { get; set; }
    public string? TrainerProfessionalExperience { get; set; }
    public string? TrainerPosition { get; set; }
    public string? Addresses { get; set; }
}