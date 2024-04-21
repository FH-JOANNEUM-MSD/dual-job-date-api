namespace DualJobDate.BusinessObjects.Entities.Interface;

public interface IRegisterUserFromJsonModel
{
    public string Email { get; set; }
    public int AcademicProgramYear { get; set; }
    public string AcademicProgramKeyName { get; set; }
    public string InstitutionKeyName { get; set; }
}