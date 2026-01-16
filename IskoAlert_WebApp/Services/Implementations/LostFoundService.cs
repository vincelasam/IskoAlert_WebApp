using IskoAlert_WebApp.Data;
using IskoAlert_WebApp.Models.Domain;
using IskoAlert_WebApp.Models.Domain.Enums;
using IskoAlert_WebApp.Models.ViewModels.LostFound;
using IskoAlert_WebApp.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace IskoAlert_WebApp.Services.Implementations
{
    public class LostFoundService : ILostFoundService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public LostFoundService(ApplicationDbContext context)
        {
            _context = context;
        }

        public LostFoundService(IWebHostEnvironment webHostEnvironment)
        {
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



        Task async UpdateItemStatusAsync(int itemId, ReportStatus newStatus, int userId)
        {

        }

        Task async <List<LostFoundItem>> GetAllItemsAsync(string keyword)
        {

        }

        Task async <List<LostFoundItem>> GetUserItemsAsync(int userId)
        {

        }

        Task async <LostFoundItem> GetItemByIdAsync(int itemId)
        {

        }

        Task async DeleteItemAsync(int itemId, int userId)
        {

        }
    }

}