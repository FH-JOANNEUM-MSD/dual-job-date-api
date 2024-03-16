using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface;
using Microsoft.IdentityModel.Tokens;

namespace DualJobDate.BusinessLogic.Helper
{
    public class JwtHelper(string secretKey) : IJwtHelper
    {
        private readonly string _secretKey = secretKey;

        public string GenerateJwtToken(string email, int? expiresInMinute = null)
        {
            var key = Encoding.ASCII.GetBytes(_secretKey);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, email) }),
                Expires = DateTime.UtcNow.AddMinutes(expiresInMinute ?? Constants.JwtConstants.ExpiresInMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescription);
            return tokenHandler.WriteToken(token);
        }
    }
}
