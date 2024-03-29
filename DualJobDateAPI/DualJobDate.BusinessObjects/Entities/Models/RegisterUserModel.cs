using DualJobDate.BusinessObjects.Entities.Enum;

namespace DualJobDate.BusinessObjects.Entities.Models
{
    public class RegisterUserModel
    {
        public required string Email { get; set; }
        public required UserTypeEnum Role { get; set; }
        public int? InstitutionId { get; set; }
        public int? AcademicProgramId { get; set; }
        public int? CompanyId { get; set; }
    }
}

