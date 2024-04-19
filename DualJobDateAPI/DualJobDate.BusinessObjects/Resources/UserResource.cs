using DualJobDate.BusinessObjects.Entities.Enum;

namespace DualJobDate.BusinessObjects.Resources;

public class UserResource
{
    public string Id { get; set; }
    public int? AcademicProgramId { get; set; }
    public int? InstitutionId { get; set; }
    public bool IsActive { get; set; }
    public UserTypeEnum UserType { get; set; }
    public bool IsNew { get; set; }
    public string Email { get; set; }
    public UserCompanyResource Company { get; set; }
}