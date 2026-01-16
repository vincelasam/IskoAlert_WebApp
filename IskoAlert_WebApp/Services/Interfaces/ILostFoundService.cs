using IskoAlert_WebApp.Models.Domain;
using IskoAlert_WebApp.Models.Domain.Enums;
using IskoAlert_WebApp.Models.ViewModels.LostFound;

public interface ILostFoundService
{
    Task CreateLostItemAsync(LostFoundItemViewModel model);

    Task<List<LostFoundItem>> GetAllItemsAsync();

    Task<List<LostFoundItem>> GetUserItemsAsync(int userId);

    Task<LostFoundItem?> GetItemByIdAsync(int itemId);

    Task UpdateItemStatusAsync(int itemId, ItemStatus newStatus);

    Task DeleteItemAsync(int itemId);
}

