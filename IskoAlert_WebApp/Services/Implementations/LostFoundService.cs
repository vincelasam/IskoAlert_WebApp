using IskoAlert_WebApp.Data;
using IskoAlert_WebApp.Models.Domain;
using IskoAlert_WebApp.Models.ViewModels.LostFound;
using IskoAlert_WebApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using IskoAlert_WebApp.Models.Domain.Enums;

namespace IskoAlert_WebApp.Services.Implementations
{
    public class LostFoundService : ILostFoundService
    {
        private readonly ApplicationDbContext _context;

        public LostFoundService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateLostItemAsync(LostFoundItemViewModel model)
        {
            // TODO: implement creation logic
            throw new NotImplementedException();
        }

        public async Task<List<LostFoundItem>> GetAllItemsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<List<LostFoundItem>> GetUserItemsAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public async Task<LostFoundItem?> GetItemByIdAsync(int itemId)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateItemStatusAsync(int itemId, ItemStatus newStatus)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteItemAsync(int itemId)
        {
            throw new NotImplementedException();
        }
    }


}