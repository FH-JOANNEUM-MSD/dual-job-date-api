using System.ComponentModel.DataAnnotations;

namespace DualJobDate.BusinessObjects.Entities.Models;

public class RegisterStudentModel
{
    [EmailAddress]
    public string Email { get; set; }
}