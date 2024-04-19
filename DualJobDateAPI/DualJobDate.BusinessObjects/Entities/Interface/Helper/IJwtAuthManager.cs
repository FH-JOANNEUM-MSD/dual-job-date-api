using System.Security.Claims;
using DualJobDate.BusinessObjects.Entities.Models;

namespace DualJobDate.BusinessObjects.Entities.Interface.Helper;

public interface IJwtAuthManager
{
    public Task<JwtAuthResultViewModel> GenerateTokens(User user, DateTime now);
    public ClaimsPrincipal GetPrincipalFromToken(string token, bool checkExpired);
}