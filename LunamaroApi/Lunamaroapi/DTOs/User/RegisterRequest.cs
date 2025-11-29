using System.ComponentModel.DataAnnotations;

namespace Lunamaroapi.DTOs.userDTO
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Full name is required")]
        public string FulName { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [MinLength(4, ErrorMessage = "Username must be at least 4 characters")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ComfirmPassword { get; set; }
    }
}
