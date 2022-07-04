using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Lessons.ClientCardApi.Runner.Controllers
{
    [AllowAnonymous]
    [EnableCors("HealthRequestPolicy")]
    [Route("api/health")]
    public class HealthController : ControllerBase
    {
        [HttpGet("status")]
        public IActionResult Status()
        {
            return Ok("Alive");
        }
    }
}