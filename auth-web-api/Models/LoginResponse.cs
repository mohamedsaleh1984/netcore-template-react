namespace AuthWebApi.Models
{
    public class LoginResponse
    {
        public string AccessToken { get; set; } = "";
        public string RefreshToken { get; set; } = "";
    }
    public class RefreshTokenResponse
    {
        public string AccessToken { get; set; } = "";
        public RefreshToken RefreshToken { get; set; } = new RefreshToken();
    }
}
