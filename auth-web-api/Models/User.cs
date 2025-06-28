using Microsoft.AspNetCore.Identity;

namespace AuthWebApi.Models
{
    public class User : IdentityUser
    {
        public string Role { get; set; } = string.Empty;
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
