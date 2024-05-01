using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface.Helper;
using DualJobDate.BusinessObjects.Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace DualJobDate.BusinessLogic.Helper
{
    public class JwtAuthManager(UserManager<User> userManager, byte[] secret,  string issuer, string audience, int expireDate1, int expireDate2) : IJwtAuthManager
    {
        public async Task<JwtAuthResultViewModel> GenerateTokens(User user, DateTime now)
        {
            var accessToken = await CreateAccessToken(user, now);
            var refreshToken = CreateRefreshToken(user, now);

            var jwtResult = new JwtAuthResultViewModel
            {
                TokenType = "Bearer",
                AccessToken = new JwtSecurityTokenHandler().WriteToken(accessToken),
                ExpiresIn = expireDate1,
                RefreshToken = new JwtSecurityTokenHandler().WriteToken(refreshToken),
                IsNew = user.IsNew,
                UserId = user.Id,
                Email = user.Email,
                UserType = user.UserType
            };
            return jwtResult;
        }

        private async Task<JwtSecurityToken> CreateAccessToken(User user, DateTime now)
        {
            var userClaims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
            };

            var userRoles = await userManager.GetRolesAsync(user);
            userClaims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            return new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: userClaims,
                expires: now.AddSeconds(expireDate1),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(secret),
                    SecurityAlgorithms.HmacSha256)
            );
        }

        private JwtSecurityToken CreateRefreshToken(User user, DateTime now)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
            };

            return new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: now.AddDays(expireDate2),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(secret),
                    SecurityAlgorithms.HmacSha256)
            );
        }
        
        public ClaimsPrincipal GetPrincipalFromToken(string token, bool checkExpired)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secret),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = audience,
                ValidAudience = issuer,
                ValidateLifetime = checkExpired,
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
                if (checkExpired) throw;
                tokenValidationParameters.ValidateLifetime = false;
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);
                return principal;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
    
}
