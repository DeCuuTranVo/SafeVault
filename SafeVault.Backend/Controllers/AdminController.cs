using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SafeVault.Backend.Controllers
{
    [Authorize(Policy = "AdminOnly")] // Apply the AdminOnly policy
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        [HttpGet("dashboard")]
        public IActionResult GetAdminDashboard()
        {
            return Ok("Welcome to the Admin Dashboard!");
        }

        [HttpGet("settings")]
        public IActionResult GetAdminSettings()
        {
            return Ok("Here are the admin settings.");
        }
    }
}
