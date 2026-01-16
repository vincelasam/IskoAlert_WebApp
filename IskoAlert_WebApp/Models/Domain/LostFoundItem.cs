    using IskoAlert_WebApp.Models.Domain.Enums;
    using Microsoft.AspNetCore.Identity;
    using System.Data;
    namespace IskoAlert_WebApp.Models.Domain
    {
        public class LostFoundItem
        {
            public int ItemId { get; private set; }
             // Foreign Key pointing to User.UserId
        public int UserId { get; private set; }

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
            public string? ImagePath { get; private set; }


        private LostFoundItem() { } // EF Core needs this!

        public LostFoundItem(int userId, string title, string description, string email, CampusLocation location, ItemCategory category)
            {
                if (userId <= 0)
                    throw new ArgumentException("A valid UserId is required.");
                if (string.IsNullOrWhiteSpace(title))
                    throw new ArgumentException("Item name is required.");
                if (string.IsNullOrWhiteSpace(description))
                    throw new ArgumentException("Description is required.");
                if (string.IsNullOrWhiteSpace(email))
                    throw new ArgumentException("Email is required.");
                
                UserId = userId;    
                Title = title;
                Description = description;
                Email = email;
                LocationFound = location;
                Category = category;

                Status = ItemStatus.Lost;
                DatePosted = DateTime.UtcNow;
            }

            public void StatusChange(ItemStatus newStatus)
            {
                if (newStatus == ItemStatus.Found)
                    throw new InvalidOperationException("Item already found.");
                if (newStatus == ItemStatus.Lost)
                    throw new InvalidOperationException("Item already lost.");
                if (newStatus == ItemStatus.Archived)
                    throw new InvalidOperationException("Item already archived.");

                Status = newStatus;
            }

            public void UpdateImage(string imagePath)
            {
                ImagePath = imagePath;
            }

        }
    }
