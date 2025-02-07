using AuthWebApi.Helper;
using AuthWebApi.Models;
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
        private readonly TokenHelper _tokenHelper;

        public AuthController(IConfiguration config,TokenHelper tokenHelper)
        {
            _config = config;
            _tokenHelper = tokenHelper;
        }

        [HttpPost("login")]
        public ActionResult<LoginResponse> Login([FromForm] LoginModel userLogin)
        {
            string ipAddress = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            var user = Authenticate(userLogin);
            if (user == null) return Unauthorized();

            var accessToken = _tokenHelper.GenerateAccessToken(user);
            var refreshToken = _tokenHelper.GenerateRefreshToken(ipAddress);

            // Save the refresh token to the user
            user.RefreshTokens.Add(refreshToken);
            // In a real app, save the user to the database here

            return Ok(new LoginResponse()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token
            });
        }



        [HttpPost("refreshtoken")]
        public ActionResult<RefreshTokenResponse> AuthRefreshToken([FromBody] RefreshTokenRequest request)
        {
            string ipAddress = Request.HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "Unknown";

            var user = DbContext.DbContext.Users.FirstOrDefault(u => u.AccessToken.Any(t => t == request.Token));
            if (user == null) return Unauthorized("Invalid token");

            var refreshToken = user.RefreshTokens.SingleOrDefault(rt => rt.Token == request.Token);
            if (refreshToken == null) return Unauthorized("Invalid token");

            // Replace the old refresh token with a new one
            var newRefreshToken = _tokenHelper.GenerateRefreshToken(ipAddress);
            user.RefreshTokens.Add(newRefreshToken);

            // Generate a new access token
            var accessToken = _tokenHelper.GenerateAccessToken(user);

            return Ok(new RefreshTokenResponse()
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken
            });
        }

        private User? Authenticate(LoginModel userLogin)
        {
            // Replace with your user authentication logic
            var currentUser = DbContext.DbContext.Users.FirstOrDefault(u => u.Username == userLogin.Username && u.Password == userLogin.Password);
            return currentUser;
        } 
    }
}
