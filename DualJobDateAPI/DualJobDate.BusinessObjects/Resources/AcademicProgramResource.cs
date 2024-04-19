namespace DualJobDate.BusinessObjects.Resources;

public class AcademicProgramResource
{
    public string Name { get; set; } = string.Empty;
    public string KeyName { get; set; } = string.Empty;
    public AcademicDegreeEnum AcademicDegreeEnum { get; set; } = AcademicDegreeEnum.Default;
    public ICollection<UserResource> Users { get; set; } = [];
    public ICollection<ActivityResource> Activities { get; set; } = [];
    public ICollection<CompanyResource> Companies { get; set; } = [];
}