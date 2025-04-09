using Microsoft.AspNetCore.Mvc;

namespace SafeVault.Backend.Controllers
{
    [ApiController]
    [Route("api/home")]
    public class HomeController : ControllerBase
    {
        // GET: api/home/index
        [HttpGet("index")]
        public IActionResult Index()
        {
            return Ok(new { Message = "This is the home page" });
        }
    }
}
