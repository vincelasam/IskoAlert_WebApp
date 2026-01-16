using IskoAlert_WebApp.Models.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace IskoAlert_WebApp.Models.ViewModels.LostFound
{
    public class EditItem
    {
        public int UserId { get; set; }
        public int ItemId { get; set; }
        [Required]
        public ItemStatus LostOrFound { get; set; } // for radiobutton if lost or not

        [Required(ErrorMessage = "Title is required")]
        [StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Maximum of 500 characters only")]
        public string Description { get; set; } = string.Empty;
        [Required]
        public CampusLocation SelectedCampusLocation { get; set; }
        public IEnumerable<SelectListItem> CampusLocations { get; set; } = new List<SelectListItem>();
        [Required]
        public ItemCategory SelectedCategory { get; set; }
        public IEnumerable<SelectListItem> Category { get; set; } = new List<SelectListItem>();

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Upload Image")]
        public IFormFile? ImageFile { get; set; }

    }
}
