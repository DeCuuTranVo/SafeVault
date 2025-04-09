using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using SafeVault.Backend.Data;
using SafeVault.Backend.Models;

namespace SafeVault.Backend.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        // Authenticate user by verifying username or email and password
        public async Task<bool> AuthenticateUser(string usernameOrEmail, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == usernameOrEmail || u.Email == usernameOrEmail);

            if (user == null) return false;

            // Verify the password
            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }

        // Register a new user with hashed password
        public async Task RegisterUser(string username, string email, string password, string role = "user")
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = passwordHash,
                Role = role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetUserByUsernameOrEmail(string usernameOrEmail)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == usernameOrEmail || u.Email == usernameOrEmail);
        }
    }

}
