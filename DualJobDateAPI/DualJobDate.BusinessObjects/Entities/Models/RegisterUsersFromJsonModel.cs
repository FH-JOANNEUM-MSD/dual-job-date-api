namespace DualJobDate.BusinessObjects.Entities.Models;

public class RegisterUsersFromJsonModel
{
    public ICollection<RegisterStudentUserFromJsonModel> StudentUsers { get; set; } = [];
    public ICollection<RegisterCompanyUserFromJsonModel> CompanyUsers { get; set; } = [];
}