namespace DualJobDate.API.Models;

public class ResetPasswordModel
{
    public required string Email { get; set; }
    public required string ResetCode { get; set; }
    
    public required string NewPassword { get; set; }
}