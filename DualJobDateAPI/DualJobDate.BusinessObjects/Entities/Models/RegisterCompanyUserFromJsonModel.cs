using DualJobDate.BusinessObjects.Entities.Interface;

namespace DualJobDate.BusinessObjects.Entities.Models;

public class RegisterCompanyUserFromJsonModel : IRegisterUserFromJsonModel
{
    public required string Email { get; set; }
    public required string CompanyName { get; set; }
}