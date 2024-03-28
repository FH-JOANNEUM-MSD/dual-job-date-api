using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Enum;

namespace DualJobDate.BusinessObjects.Resources
{
    public class UserResource
    {
        public string Id { get; set; }
        public AcademicProgram? AcademicProgram { get; set; }
        public Institution? Institution { get; set; }
        public bool IsActive { get; set; }
        public UserTypeEnum UserType { get; set; }
        public bool IsNew { get; set; }
        public string Email { get; set; }
    }
}