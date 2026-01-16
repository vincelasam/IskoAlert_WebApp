    using IskoAlert_WebApp.Models.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace IskoAlert_WebApp.Models.Domain
{
    public class LostFoundItem
    {
        [Key]
        public int ItemId { get; private set; }
        public int UserId { get; private set; } // Foreign Key pointing to User.UserId
        // Navigation Property
        // This links this report to the User entity in the database
        public User User { get; private set; }

        public string Title { get; private set; }
        public string Description { get; private set; }
        public string Email { get; private set; }
        public CampusLocation LocationFound { get; private set; }
        public DateTime DatePosted { get; private set; }
        public ItemCategory Category { get; private set; }
        public ItemStatus Status { get; private set; }
        public DateTime? ArchivedAt { get; set; }
        public string ImagePath { get; private set; }

        protected LostFoundItem() { }

        public LostFoundItem(string title, string description,ItemStatus status, string email, CampusLocation location, ItemCategory category,string image)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Item name is required.");
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description is required.");
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.");
            if (string.IsNullOrWhiteSpace(image)) throw new ArgumentException("Image is required.");
            Title = title;
            Description = description;
            Email = email;
            LocationFound = location;
            Category = category;
            Status = status;
            ImagePath = image;
            DatePosted = DateTime.UtcNow;
        }

        public void ChangeStatus(ItemStatus newStatus)
        {
            if (Status == newStatus)
                throw new InvalidOperationException($"Item is already {newStatus}.");

            Status = newStatus;
        }

        public void Archive(int userId)
        {
            if (Status == ItemStatus.Archived)
                throw new InvalidOperationException("Item already archived.");
            if (UserId != userId)
                throw new UnauthorizedAccessException("You cannot archive this item.");


            Status = ItemStatus.Archived;
            ArchivedAt = DateTime.UtcNow;
        }

        public void UpdateImage(string imagePath) => ImagePath = imagePath;

        public void Update(string title, string description, CampusLocation location, ItemCategory category, ItemStatus status)
        {
            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required.");
            if (string.IsNullOrWhiteSpace(description)) throw new ArgumentException("Description is required.");

            Title = title;
            Description = description;
            LocationFound = location;
            Category = category;
            Status = status;
        }
    }
}
