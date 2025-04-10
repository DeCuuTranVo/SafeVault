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
            // Entity Framework Core automatically apply parameterized queries to prevent SQL injection
            var user = await _context.Users
                .AsNoTracking() // Prevent unintentional modifications
                .FirstOrDefaultAsync(u => u.Username == usernameOrEmail || u.Email == usernameOrEmail);

            if (user == null) return false;

            // Verify the password securely
            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }

        // Register a new user with hashed password
        public async Task RegisterUser(string username, string email, string password, string role = "user")
        {
            if (await _context.Users.AnyAsync(u => u.Username == username || u.Email == email))
            {
                throw new InvalidOperationException("Username or email already exists");
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12); // Increase work factor for stronger hashing
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
