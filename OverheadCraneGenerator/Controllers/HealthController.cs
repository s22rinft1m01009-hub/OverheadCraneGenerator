using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OverheadCraneGenerator.Controllers
{
    [Route("api/[controller]")]
    [Route("health")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                Status = "ok",
                Application = "Overhead Crane Generator",
                Version = "1.0.0",
                Timestamp = DateTimeOffset.UtcNow
            });
        }
    }
}
