using System.ComponentModel.DataAnnotations;

namespace SafeVault.Frontend.Models
{
    public class UserLoginModel
    {
        [Required(ErrorMessage = "Username or Email is required.")]
        [StringLength(100, ErrorMessage = "Username must be less than 100 characters.")]
        public string Username { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [StringLength(100, ErrorMessage = "Email must be less than 100 characters.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, ErrorMessage = "Password must be less than 100 characters.")]
        public string Password { get; set; }
    }
}
