using System.ComponentModel.DataAnnotations;

namespace IskoAlert_WebApp.Models.ViewModels.Account
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@iskolarngbayan\.pup\.edu\.ph$", ErrorMessage = "Format must be @iskolarngbayan.pup.edu.ph (e.g. juantamad@iskolarngbayan.pup.edu.ph")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; } = string.Empty;
    }
}
