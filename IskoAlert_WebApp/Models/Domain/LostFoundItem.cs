using IskoAlert_WebApp.Models.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System.Data;
namespace IskoAlert_WebApp.Models.Domain
{
    public class LostFoundItem
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Email { get; private set; }
        public CampusLocation LocationFound { get; private set; }
        public DateTime DatePosted { get; private set; }
        public ItemCategory Category { get; private set; }
        public ItemStatus Status { get; private set; }
        public DateTime? ArchivedAt { get; set; }



        public LostFoundItem(string name, string description, string email, CampusLocation location, ItemCategory category)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Item name is required.");
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description is required.");
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.");

            Name = name;
            Description = description;
            Email = email;
            LocationFound = location;
            Category = category;

            Status = ItemStatus.Lost;
            DatePosted = DateTime.UtcNow;
        }


    }
}
