using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Requires authentication
    public class ProtectedController : ControllerBase
    {
        public async Task<ActionResult<string>> Call()
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
