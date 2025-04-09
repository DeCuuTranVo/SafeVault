using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SafeVault.Backend.Data;

namespace SafeVault.Backend.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/users/index
        [Authorize]
        [HttpGet("index")]
        public IActionResult Index()
        {
            return Ok(new { Message = "This is the user page" });
        }

        // GET: api/users
        [Authorize] // Protect this endpoint
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users
                .Select(u => new { u.UserID, u.Username, u.Email, u.Role })
                .ToListAsync();

            return Ok(users);
        }

        // GET: api/users/{id}
        [Authorize] // Protect this endpoint
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _context.Users
                .Select(u => new { u.UserID, u.Username, u.Email, u.Role })
                .FirstOrDefaultAsync(u => u.UserID == id);

            if (user == null)
                return NotFound(new { Message = "User not found" });

            return Ok(user);
        }
    }
}
