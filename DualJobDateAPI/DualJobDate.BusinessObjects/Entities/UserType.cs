using DualJobDate.BusinessObjects.Entities.Base;
using DualJobDate.BusinessObjects.Entities.Enum;

namespace DualJobDate.BusinessObjects.Entities;

public class UserType : BaseEntity
{
    public string Name { get; set; }
    public UserTypeEnum UserTypeEnum { get; set; }
}