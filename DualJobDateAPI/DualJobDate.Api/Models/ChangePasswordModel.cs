namespace DualJobDate.API.Models;

public class ChangePasswordModel
{
    public required string OldPassword { get; set; }
    public required string NewPassword { get; set; }
}