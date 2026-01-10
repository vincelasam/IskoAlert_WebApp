using System.ComponentModel.DataAnnotations;

namespace IskoAlert_WebApp.Models.ViewModels.Account
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Student ID is required")]
        [RegularExpression(@"^\d{4}-\d{5}-\w{2}-\d{1}$", ErrorMessage = "Format must be YYYY-XXXXX-MN-0 (e.g. 2020-12345-MN-0")]
        public string IdNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Webmail is required")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@iskolarngbayan\.pup\.edu\.ph$", ErrorMessage = "Format must be @iskolarngbayan.pup.edu.ph (e.g. juantamad@iskolarngbayan.pup.edu.ph")]
        [EmailAddress]
        public string Webmail { get; set; }

        [Required]
        [MinLength(8)]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}
