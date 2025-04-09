using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SafeVault.Backend.Models;
using SafeVault.Backend.Models.Dto;
using SafeVault.Backend.Services;
using System.Security.Claims;
using System.Text;

namespace SafeVault.Backend.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly IConfiguration _configuration;
        public AuthController(AuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto userDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _authService.RegisterUser(userDto.Username, userDto.Email, userDto.Password, userDto.Role);
            return Ok(new { Message = "User registered successfully" });
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _authService.GetUserByUsernameOrEmail(loginDto.UsernameOrEmail);
            if (user == null || !await _authService.AuthenticateUser(loginDto.UsernameOrEmail, loginDto.Password))
                return Unauthorized(new { Message = "Invalid credentials" });

            // Generate JWT token
            var token = GenerateJwtToken(user);

            return Ok(new
            {
                Message = "Login successful",
                Token = token
            });
        }

        // Helper method to generate JWT token
        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            if (key.Length < 32)
            {
                throw new ArgumentOutOfRangeException(nameof(key), "The JWT signing key must be at least 256 bits (32 bytes).");
            }
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
               new Claim(ClaimTypes.Name, user.Username),
               new Claim(ClaimTypes.Email, user.Email),
               new Claim(ClaimTypes.Role, user.Role)
           }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
