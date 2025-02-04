
using Microsoft.AspNetCore.Mvc;
namespace AuthWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost("login")]
    public IActionResult Login([FromForm] LoginModel login)
    {
        // Replace with actual user validation logic
        if (login.Username == "admin" && login.Password == "password")
        {
            var token = TokenHelper.GenerateJwtToken(login.Username, _configuration);
            return Ok(new { Token = token });
        }
        return Unauthorized();
    }
}

public class LoginModel
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
}
