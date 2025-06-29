

using AuthWebApi.AppDbContext;
using AuthWebApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuthWebApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
            _roleManager = roleManager;
        }

        public async Task<TokenResponseDto?> LoginAsync(UserDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == request.Username);
            if (user is null)
            {
                return null;
            }
            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash!, request.Password)
                == PasswordVerificationResult.Failed)
            {
                return null;
            }

            return await CreateTokenResponse(user);
        }

        private async Task<TokenResponseDto> CreateTokenResponse(User? user)
        {
            if (user == null)
            {
                return new TokenResponseDto()
                {
                    AccessToken = "",
                    RefreshToken = ""
                };
            }

            return new TokenResponseDto
            {
                AccessToken = await CreateToken(user),
                RefreshToken = await GenerateAndSaveRefreshTokenAsync(user)
            };
        }

        public async Task<User?> RegisterAsync(UserDto request)
        {
            if (await _context.Users.AnyAsync(u => u.UserName == request.Username))
            {
                return null;
            }

            string[] roles = ["Admin", "Manager", "User"];
            // Add Role
            int roleIndex = new Random().Next(0, 3);
            string roleName = roles[roleIndex];

            var user = new User();
            var hashedPassword = new PasswordHasher<User>().HashPassword(user, request.Password);

            user.UserName = request.Username;
            user.PasswordHash = hashedPassword;
            user.Email = request.Username;
            user.SecurityStamp = Guid.NewGuid().ToString();

            _context.Users.Add(user);
            int save_result = await _context.SaveChangesAsync();
            if (save_result > 0)
            {
                var role_result = await _userManager.AddToRoleAsync(user, roleName);
                if (!role_result.Succeeded)
                {
                    return null;
                }
            }

            return user;
        }

        public async Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request)
        {
            var user = await ValidateRefreshTokenAsync(request.UserId, request.RefreshToken);
            if (user is null)
                return null;

            return await CreateTokenResponse(user);
        }

        private async Task<User?> ValidateRefreshTokenAsync(Guid userId, string refreshToken)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user is null || user.RefreshToken != refreshToken
                || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return null;
            }

            return user;
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private async Task<string> GenerateAndSaveRefreshTokenAsync(User user)
        {
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_configuration.GetValue<int>("JwtSettings:RefreshTokenExpires"));

            await _context.SaveChangesAsync();
            return refreshToken;
        }

        private async Task<string> CreateToken(User user)
        {
            // Add roles to the claims
            var userRoles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration.GetSection("JwtSettings").GetValue<string>("Token")!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _configuration.GetValue<string>("JwtSettings:Issuer"),
                audience: _configuration.GetValue<string>("JwtSettings:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("JwtSettings:AccessTokenExpires")),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}
