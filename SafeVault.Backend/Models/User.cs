
using System.ComponentModel.DataAnnotations;

namespace SafeVault.Backend.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; } // Primary Key

        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty; // Required, Max Length 100

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty; // Required, Max Length 100, Valid Email

        // Store hashed passwords
        public string PasswordHash { get; set; }

        public string Role { get; set; } // For role-based authorization
    }

}
