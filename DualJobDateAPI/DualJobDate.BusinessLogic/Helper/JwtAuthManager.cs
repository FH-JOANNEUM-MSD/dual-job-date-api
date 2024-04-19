using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DualJobDate.BusinessObjects.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DualJobDate.BusinessLogic.Helper
{
    public class JwtAuthManager
{
    public UserManager<User> userManager { get; }
    private readonly byte[] secret;
    private readonly IConfiguration configuration;

    public JwtAuthManager(
        UserManager<User> userManager,
        IConfiguration configuration)
    {
        secret = Encoding.ASCII.GetBytes(configuration["JwtSecret"]);
        this.userManager = userManager;
    }

    public async Task<JwtAuthResultViewModel> GenerateTokens(User user, DateTime now)
    {
        var accessToken = await CreateAccessToken(user, now);
        var refreshToken = CreateRefreshToken(user, now);

        return new JwtAuthResultViewModel
        {
            TokenType = "Bearer",
            AccessToken = new JwtSecurityTokenHandler().WriteToken(accessToken),
            ExpiresIn = now.AddMinutes(1),
            RefreshToken = new JwtSecurityTokenHandler().WriteToken(refreshToken),
            IsNew = user.IsNew
        };
    }

    private async Task<JwtSecurityToken> CreateAccessToken(User user, DateTime now)
    {
        var userClaims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
        };

        var userRoles = await userManager.GetRolesAsync(user);
        userClaims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

        return new JwtSecurityToken(
            issuer: "localhost",
            audience: "localhost",
            claims: userClaims,
            expires: now.AddMinutes(1),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(secret),
                SecurityAlgorithms.HmacSha256Signature)
        );
    }

    private JwtSecurityToken CreateRefreshToken(User user, DateTime now)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
        };

        return new JwtSecurityToken(
            issuer: "localhost",
            audience: "localhost",
            claims: claims,
            expires: now.AddDays(7),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(secret),
                SecurityAlgorithms.HmacSha256Signature)
        );
    }
    
    public ClaimsPrincipal GetPrincipalFromToken(string token, bool checkExpired = false)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(secret),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = "localhost",
            ValidAudience = "localhost",
            ValidateLifetime = checkExpired, // Hier können Sie steuern, ob das Ablaufdatum überprüft werden soll.
            ClockSkew = TimeSpan.Zero
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);
            return principal;
        }
        catch (SecurityTokenExpiredException)
        {
            if (!checkExpired)
            {
                tokenValidationParameters.ValidateLifetime = false;
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);
                return principal;
            }
            throw;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

}
    public class JwtAuthResultViewModel
    {
        public string TokenType { get; set; }
        public string AccessToken { get; set; }
        public DateTime ExpiresIn { get; set; }
        public string RefreshToken { get; set; }
        public bool IsNew { get; set; }
    }
}