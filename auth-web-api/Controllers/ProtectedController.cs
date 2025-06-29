using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class ProtectedController : ControllerBase
    {
        [Authorize] // Requires authentication
        [HttpGet("no-role")]
        public async Task<ActionResult<string>> AuthUserNoRole()
        {
            int res = await Task.Run(() =>
            {
                // Simulate some processing that takes time
                Task.Delay(2000).Wait(); // Simulate some delay
                return 1 + 1;
            }); // Simulate some processing

            // This endpoint is protected and requires authentication
            return Ok("You have accessed a protected endpoint!, Calc result is " + res.ToString());
        }


        [Authorize(Roles = "Admin,Manager")]
        [HttpGet("auth-admin-manager")]
        public async Task<ActionResult<string>> AdminManager()
        {
            int res = await Task.Run(() =>
            {
                // Simulate some processing that takes time
                Task.Delay(2000).Wait(); // Simulate some delay
                return 1 + 1;
            }); // Simulate some processing

            // This endpoint is protected and requires authentication
            return Ok("You have accessed a protected endpoint!, Calc result is " + res.ToString());
        }


        [Authorize(Roles = "Admin,Bartender")]
        [HttpGet("auth-admin-bartender")]
        public async Task<ActionResult<string>> AdminBartender()
        {
            int res = await Task.Run(() =>
            {
                // Simulate some processing that takes time
                Task.Delay(2000).Wait(); // Simulate some delay
                return 1 + 1;
            }); // Simulate some processing

            // This endpoint is protected and requires authentication
            return Ok("You have accessed a protected endpoint!, Calc result is " + res.ToString());
        }

        [Authorize(Roles = "User,Bartender")]
        [HttpGet("auth-bartender")]
        public async Task<ActionResult<string>> UserBartender()
        {
            int res = await Task.Run(() =>
            {
                // Simulate some processing that takes time
                Task.Delay(2000).Wait(); // Simulate some delay
                return 1 + 1;
            }); // Simulate some processing

            // This endpoint is protected and requires authentication
            return Ok("You have accessed a protected endpoint!, Calc result is " + res.ToString());
        }

        [Authorize(Roles = "Postman")]
        [HttpGet("auth-postman")]
        public async Task<ActionResult<string>> Postman()
        {
            int res = await Task.Run(() =>
            {
                // Simulate some processing that takes time
                Task.Delay(2000).Wait(); // Simulate some delay
                return 1 + 1;
            }); // Simulate some processing

            // This endpoint is protected and requires authentication
            return Ok("You have accessed a protected endpoint!, Calc result is " + res.ToString());
        }
    }
}
