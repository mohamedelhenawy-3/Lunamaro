using System.ComponentModel.DataAnnotations;

namespace Lunamaroapi.DTOs.userDTO
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Full name is required")]
        [MinLength(15, ErrorMessage = "Full name must be at least 15 characters")]
        public string FullName { get; set; }


        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        [RegularExpression(
    @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@#$%^&+=!]).{6,}$",
    ErrorMessage = "Password must contain uppercase 'A' , lowercase 'a' , number, and special character '@#_$!' "
)]
     
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}
