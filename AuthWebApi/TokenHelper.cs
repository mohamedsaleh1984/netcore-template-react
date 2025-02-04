using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

public static class TokenHelper
{
    public static string GenerateJwtToken(string username, IConfiguration configuration)
    {
        Console.WriteLine("Generate JWT.");
        var jwtSettings = configuration.GetSection("JwtSettings");
        string jwtKey = jwtSettings["Key"] ?? "";

        if (string.IsNullOrEmpty(jwtKey))
            throw new Exception("JWT Key is not found in appsettings.json");

        var key = Encoding.UTF8.GetBytes(jwtKey);

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, username)
            }),
            Expires = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpiresInMinutes"] ?? "")),

            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        Console.WriteLine("Token Got generated successfully.");
        return tokenHandler.WriteToken(token);
    }
}
