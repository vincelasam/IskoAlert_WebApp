using IskoAlert_WebApp.Models.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace IskoAlert_WebApp.Models.ViewModels.LostFound
{
    public class LostFoundItemDisplayViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string DescriptionPreview { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public string CampusLocation { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DateTime DatePosted { get; set; }

        public string? ImagePath { get; set; }
        public string Email { get; set; }

    }
}
