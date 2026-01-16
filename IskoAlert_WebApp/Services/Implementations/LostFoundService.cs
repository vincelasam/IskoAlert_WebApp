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

        public async Task CreateLostItemAsync(CreateItem model, int userId)
        {
            string imagePath = await SaveImageAsync(model.ImageFile); 

            var newItem = new LostFoundItem(
                userId: userId,
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

        public async Task UpdateItemAsync(EditItem model, int userId)
        {
            var item = await _context.LostFoundItems
              .FirstOrDefaultAsync(x => x.ItemId == model.ItemId);
            // Check if item exists
            if (item == null)
                throw new KeyNotFoundException("Item not found.");

            string imagePath = item.ImagePath;
            if (model.ImageFile != null)
            {
                imagePath = await SaveImageAsync(model.ImageFile);
            }

            // Check if the current user is allowed to archive it
            if (item.UserId != userId)
                throw new UnauthorizedAccessException("You cannot archive this item.");

            item.Update(
                title: model.Title,
                description: model.Description,
                status: model.LostOrFound,
                email: model.Email,
                location: model.SelectedCampusLocation,
                category: model.SelectedCategory,
                image: imagePath
                );

            await _context.SaveChangesAsync();
        }

        public async Task<List<LostFoundItem>> GetAllItemsAsync(string? keyword, string? status = null)
        {
            var query = _context.LostFoundItems
                .AsNoTracking()
                .Where(i => i.ArchivedAt == null)
                .AsQueryable();

            // Filter by status if provided
            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<ItemStatus>(status, out var statusEnum))
            {
                query = query.Where(i => i.Status == statusEnum);
            }

            // Search by keyword
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var keywordLower = keyword.ToLower();

                // Search in Title and Description
                query = query.Where(i =>
                    EF.Functions.Like(i.Title, $"%{keyword}%") ||
                    EF.Functions.Like(i.Description, $"%{keyword}%")
                );

                var matchingLocations = Enum.GetValues(typeof(CampusLocation))
                    .Cast<CampusLocation>()
                    .Where(loc => loc.ToString().Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (matchingLocations.Any())
                {
                    var locationQuery = query.Where(i => matchingLocations.Contains(i.LocationFound));
                    query = query.Where(i =>
                        EF.Functions.Like(i.Title, $"%{keyword}%") ||
                        EF.Functions.Like(i.Description, $"%{keyword}%") ||
                        matchingLocations.Contains(i.LocationFound)
                    );
                }
            }

            query = query.OrderByDescending(i => i.DatePosted);

            return await query.ToListAsync();
        }

        public async Task<List<LostFoundItem>> GetUserItemsAsync(int userId, bool includeArchived = false)
        {
            var query = _context.LostFoundItems
                .Where(i => i.UserId == userId)
                .AsQueryable();

            if (!includeArchived)
            {
                query = query.Where(i => i.ArchivedAt == null);
            }

            return await query
                .OrderByDescending(i => i.DatePosted)
                .ToListAsync();
        }

        public async Task<LostFoundItem?> GetItemByIdAsync(int itemId)
        {
            return await _context.LostFoundItems.FirstOrDefaultAsync(x => x.ItemId == itemId);
        }

        public async Task ArchiveItemAsync(int itemId, int userId)
        {
            // Fetch the item from the database
            var item = await _context.LostFoundItems
                .FirstOrDefaultAsync(x => x.ItemId == itemId);

            // Check if item exists
            if (item == null)
                throw new KeyNotFoundException("Item not found.");

            // Check if the current user is allowed to archive it
            if (item.UserId != userId)
                throw new UnauthorizedAccessException("You cannot archive this item.");

            item.Archive(userId);

            await _context.SaveChangesAsync();
        }

        public async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                throw new ArgumentException("Image is required.");

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
            var uploadsPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/items");

            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            var path = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return $"/uploads/items/{fileName}";
        }
    }

}