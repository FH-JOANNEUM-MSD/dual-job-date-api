namespace DualJobDate.BusinessObjects.Entities.Models;

public class JwtAuthResultViewModel
{
    public string TokenType { get; set; }
    public string AccessToken { get; set; }
    public int ExpiresIn { get; set; }
    public string RefreshToken { get; set; }
    public bool IsNew { get; set; }
}