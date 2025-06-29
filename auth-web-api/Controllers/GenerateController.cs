using AuthWebApi.Models;
using AuthWebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenerateController : ControllerBase
    {
        private readonly IAuthService auth;
        public GenerateController(IAuthService authService)
        {
            auth = authService;
        }

        [HttpPost("random-users")]
        public async Task<ActionResult> CreateRandomeUsers()
        {
            string password = "HellWorld!3";

            for (int i = 0; i < 10; i++)
            {
                UserDto user = new UserDto();
                user.Username = "".GetRandomString(5, 8) + "@hotmail.com";
                user.Password = password;
                await auth.RegisterAsync(user);
            }


            return Ok("Created");
        }
    }
}
