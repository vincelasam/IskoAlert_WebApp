using IskoAlert_WebApp.Models.Domain.Enums;
using IskoAlert_WebApp.Models.ViewModels.LostFound;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IskoAlert_WebApp.Services
{
    public static class DropdownHelper
    {
        public static void PopulateLostFoundDropdowns(dynamic model)
        {
            // Category dropdown from ItemCategory enum
            model.Category = Enum.GetValues(typeof(ItemCategory))
                                 .Cast<ItemCategory>()
                                 .Select(c => new SelectListItem
                                 {
                                     Text = c.ToString(),
                                     Value = ((int)c).ToString()
                                 }).ToList();

            // Campus location dropdown from CampusLocation enum
            model.CampusLocations = Enum.GetValues(typeof(CampusLocation))
                                        .Cast<CampusLocation>()
                                        .Select(l => new SelectListItem
                                        {
                                            Text = l.ToString(),
                                            Value = ((int)l).ToString()
                                        }).ToList();
        }
    }
}
