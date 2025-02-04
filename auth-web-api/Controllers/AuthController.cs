using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuthWebApi.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;

        public AuthController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("login")]
        public IActionResult Login([FromForm] LoginModel userLogin)
        {
            string ipAddress = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            var user = Authenticate(userLogin);
            if (user == null) return Unauthorized();

            var accessToken = GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken(ipAddress);

            // Save the refresh token to the user
            user.RefreshTokens.Add(refreshToken);
            // In a real app, save the user to the database here

            return Ok(new
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }

        [HttpPost("refreshtoken")]
        public IActionResult AuthRefreshToken([FromBody] RefreshTokenRequest request)
        {
            string ipAddress = Request.HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "Unknown";

            var user = UserConstants.Users.FirstOrDefault(u => u.AccessToken.Any(t => t == request.Token));
            if (user == null) return Unauthorized("Invalid token");

            var refreshToken = user.RefreshTokens.SingleOrDefault(rt => rt.Token == request.Token);
            if (refreshToken == null) return Unauthorized("Invalid token");

            // Replace the old refresh token with a new one
            var newRefreshToken = GenerateRefreshToken(ipAddress);
            user.RefreshTokens.Add(newRefreshToken);

            // Generate a new access token
            var accessToken = GenerateAccessToken(user);

            return Ok(new
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken
            });
        }


        private User Authenticate(LoginModel userLogin)
        {
            // Replace with your user authentication logic
            var currentUser = UserConstants.Users.FirstOrDefault(u => u.Username == userLogin.Username && u.Password == userLogin.Password);
            return currentUser;
        }
        private RefreshToken GenerateRefreshToken(string ipAddress)
        {
            return new RefreshToken
            {
                Token = Guid.NewGuid().ToString(),
                Expires = DateTime.UtcNow.AddDays(7),
                Revoked = false,
                Created = DateTime.UtcNow,
                RevokedAt = null,
                RevokedByIp = ipAddress
            };
        }
        private string GenerateAccessToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, user.Username),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role)
    };

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(15), // Short-lived access token
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public static class UserConstants
        {
            public static List<User> Users = new List<User>
                {
                    new User { Username = "admin", Email = "admin@example.com", Role = "Admin",Password="password" },
                    new User { Username = "user", Email = "user@example.com", Role = "User",Password="password" }
                };
        }
        public class User
        {
            public string Username { get; set; }
            public string Email { get; set; }
            public string Role { get; set; }
            public string Password { get; set; }
            public List<string> AccessToken { get; set; } = new List<string>();
            public List<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        }
   
        public class RefreshTokenRequest
        {
            public string Token { get; set; } = "";
        }
        public class RefreshToken
        {
            public string Token { get; set; } = "";
            public DateTime Expires { get; set; }
            public bool Revoked { get; set; }
            public DateTime Created { get; set; }

            public DateTime? RevokedAt { get; set; }
            public string RevokedByIp { get; set; } = "";
        }
        public class LoginModel
        {
            public string Username { get; set; } = "";
            public string Password { get; set; } = "";
        }
    }
}
