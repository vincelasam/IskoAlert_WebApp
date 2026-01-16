using IskoAlert_WebApp.Data;
using IskoAlert_WebApp.Models.Domain;
using IskoAlert_WebApp.Models.Domain.Enums;
using IskoAlert_WebApp.Models.ViewModels.LostFound;
using IskoAlert_WebApp.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace IskoAlert_WebApp.Services.Implementations
{
    public class LostFoundService : ILostFoundService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public LostFoundService(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task CreateLostItemAsync(CreateItem model)
        {
            var photo = model.ImageFile;
            if (photo == null || photo.Length == 0)
                throw new ArgumentException("Image is required.");
            
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(photo.FileName)}";
            var path = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/items", fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await photo.CopyToAsync(stream);
            }

            string imagePath = $"/uploads/items/{fileName}";

            var newItem = new LostFoundItem(
                title: model.Title,
                description: model.Description,
                status: model.LostOrFound,
                email: model.Email,
                location: model.SelectedCampusLocation,
                category: model.SelectedCategory,
                image: imagePath
                );
     
            //  Database operations
            _context.LostFoundItems.Add(newItem);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateItemStatusAsync(int itemId, ReportStatus newStatus, int userId)
        {

            throw new NotImplementedException();
        }

        public async Task<List<LostFoundItem>> GetAllItemsAsync(string keyword)
        {
            var query = _context.LostFoundItems
                       .Where(i => i.ArchivedAt == null)
                       .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var lowerKeyword = keyword.ToLower();

                query = query.Where(i =>
                    i.Title.ToLower().Contains(lowerKeyword) ||
                    i.Description.ToLower().Contains(lowerKeyword)
                );
            }

            query = query.OrderByDescending(i => i.DatePosted);

            return await query.ToListAsync();
        }

        public async Task<List<LostFoundItem>> GetUserItemsAsync(int userId)
        {
            return await _context.LostFoundItems
                   .Where(i => i.UserId == userId && i.ArchivedAt == null)
                   .OrderByDescending(i => i.DatePosted)
                   .ToListAsync();
        }

        public async Task<LostFoundItem?> GetItemByIdAsync(int itemId)
        {
            return await _context.LostFoundItems.FirstOrDefaultAsync(x => x.ItemId == itemId);
        }

        public async Task DeleteItemAsync(int itemId, int userId)
        {
            // Fetch the item from the database
            var item = await _context.LostFoundItems
                .FirstOrDefaultAsync(x => x.ItemId == itemId);

            // Check if item exists
            if (item == null)
                throw new Exception("Item not found.");

            // Check if the current user is allowed to archive it
            if (item.UserId != userId)
                throw new UnauthorizedAccessException("You cannot archive this item.");

            item.Archive(userId);

            await _context.SaveChangesAsync();
        }
    }

}