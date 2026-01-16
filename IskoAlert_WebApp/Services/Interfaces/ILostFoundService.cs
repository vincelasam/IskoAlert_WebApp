using IskoAlert_WebApp.Models.Domain;
using IskoAlert_WebApp.Models.Domain.Enums;
using IskoAlert_WebApp.Models.ViewModels.LostFound;

namespace IskoAlert_WebApp.Services.Interfaces
{
    public interface ILostFoundService
    {
        // Creates a new lost or found item
        Task CreateLostItemAsync(CreateItem model);

        // Retrieves all lost and found items
        Task<List<LostFoundItem>> GetAllItemsAsync(string keyword);

        // Retrieves all items reported by a specific user
        Task<List<LostFoundItem>> GetUserItemsAsync(int userId);
        Task<LostFoundItem> GetItemByIdAsync(int itemId);//by id

        // Updates the status of a specific item
        Task UpdateItemStatusAsync(int itemId, ReportStatus newStatus, int userId);

        Task DeleteItemAsync(int itemId, int userId);
    }
}