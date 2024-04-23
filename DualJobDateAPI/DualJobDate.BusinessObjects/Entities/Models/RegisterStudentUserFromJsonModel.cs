using DualJobDate.BusinessObjects.Entities.Interface;

namespace DualJobDate.BusinessObjects.Entities.Models;

public class RegisterStudentUserFromJsonModel : IRegisterUserFromJsonModel
{
    public required string Email { get; set; }
}