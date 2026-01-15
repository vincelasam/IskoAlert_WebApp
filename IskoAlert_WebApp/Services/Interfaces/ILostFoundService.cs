using IskoAlert_WebApp.Models.Domain;
using IskoAlert_WebApp.Models.Domain.Enums;
using IskoAlert_WebApp.Models.ViewModels.LostFound;

namespace IskoAlert_WebApp.Services.Interfaces
{
    public interface ILostFoundService
    {
        // Creates a new lost or found item
        Task CreateLostItemAsync(LostFoundItemViewModel model);

        // Retrieves all lost and found items
        Task<List<LostFoundItem>> GetAllItemsAsync();

        // Retrieves all items reported by a specific user
        Task<List<LostFoundItem>> GetUserItemsAsync(int userId);
        Task GetItemByIdAsync(int itemId);

        // Updates the status of a specific item
        Task UpdateItemStatusAsync(int itemId, ReportStatus newStatus);

        Task DeleteItemAsync(int itemId);
    }
}