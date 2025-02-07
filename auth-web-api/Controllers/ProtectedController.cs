using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Requires authentication
    public class ProtectedController : ControllerBase
    {
        public IActionResult Get()
        {
            return Ok(new { Message = "This is a protected endpoint!" });
        }
    }
}
