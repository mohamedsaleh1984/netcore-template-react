namespace AuthWebApi.Models
{
    public class User
    {
        public string Username { get; set; }= "";   
        public string Email { get; set; } = "";
        public string Role { get; set; } = "";
        public string Password { get; set; } = "";
        public List<string> AccessToken { get; set; } = new List<string>();
        public List<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
