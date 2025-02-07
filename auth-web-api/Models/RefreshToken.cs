namespace AuthWebApi.Models
{
    public class RefreshToken
    {
        public string Token { get; set; } = "";
        public DateTime Expires { get; set; }
        public bool Revoked { get; set; }
        public DateTime Created { get; set; }

        public DateTime? RevokedAt { get; set; }
        public string RevokedByIp { get; set; } = "";
    }
}
