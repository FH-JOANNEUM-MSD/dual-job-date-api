using DualJobDate.BusinessObjects.Entities.Enum;
using Microsoft.AspNetCore.Identity;

namespace DualJobDate.BusinessObjects.Entities;

public class Role : IdentityRole
{
    public UserTypeEnum UserTypeEnum { get; set; }
}