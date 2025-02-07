using AuthWebApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthWebApi.Helper
{
    public class TokenHelper(IConfiguration config)
    {
        private readonly IConfiguration _config = config;

        public RefreshToken GenerateRefreshToken(string ipAddress)
        {
            return new RefreshToken
            {
                Token = Guid.NewGuid().ToString(),
                Expires = DateTime.UtcNow.AddDays(Convert.ToInt32(_config["JwtSettings:RefreshTokenExpires"])),
                Revoked = false,
                Created = DateTime.UtcNow,
                RevokedAt = null,
                RevokedByIp = ipAddress
            };
        }
        public string GenerateAccessToken(User user)
        {
            var jwtKey = _config["JwtSettings:Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new ArgumentNullException(nameof(jwtKey), "JWT Key cannot be null or empty.");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                    new Claim(ClaimTypes.NameIdentifier, user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
               issuer:  _config["JwtSettings:Issuer"],
               audience: _config["JwtSettings:Audience"],
               claims: claims,
               expires: DateTime.Now.AddMinutes(Convert.ToInt32(_config["JwtSettings:AccessTokenExpires"])), // Short-lived access token
               signingCredentials: credentials);

            string tokenPayload = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenPayload;
        }
    }
}
