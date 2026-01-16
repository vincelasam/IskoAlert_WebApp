using IskoAlert_WebApp.Models.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IskoAlert_WebApp.Models.ViewModels.LostFound
{
    public class LostFoundListViewModel
    {
        // SEARCH / FILTER FIELDS
        public string? SearchKeyword { get; set; }

        public ItemStatus? SelectedStatus { get; set; }   // Lost / Found

        public CampusLocation? SelectedCampusLocation { get; set; }

        // Dropdown source
        public IEnumerable<SelectListItem> CampusLocations { get; set; }
            = new List<SelectListItem>();

        // RESULTS (READ-ONLY)
         
        public List<LostFoundItemDisplayViewModel> Items { get; set; }
            = new();
    }
}