using IskoAlert_WebApp.Models.Domain.Enums;
using Microsoft.AspNetCore.Http.HttpResults;

namespace IskoAlert_WebApp.Models.Domain
{
    public class User
    {
        public int UserId { get; private set; }
        public string IdNumber { get; private set; }
        public string Webmail { get; private set; }
        public string PasswordHash { get; private set; }
        public DateTime AccountCreated { get; private set; }
        public AccountStatus AccountStatus { get; private set; }
        public UserRole Role { get; private set; }
        public string Name { get; set; }

        // Constructor = how a VALID user is created
        public User(string idNumber, string webmail, string passwordHash, string name, UserRole role)
        {
            IdNumber = idNumber;
            Webmail = webmail;
            PasswordHash = passwordHash;
            Name = name;
            Role = role;

            AccountStatus = AccountStatus.Active;
            AccountCreated = DateTime.UtcNow;
        }
        public void Deactivate()
        {
            if (AccountStatus == AccountStatus.Inactive)
                throw new InvalidOperationException("User already inactive.");
            AccountStatus = AccountStatus.Inactive;
        }

        public void ChangePassword(string newHash)
        {
            PasswordHash = newHash;
        }

        public void ChangeRole(UserRole newRole)
        {
            if (Role == UserRole.Admin && newRole == UserRole.Student)
                throw new InvalidOperationException("Admin cannot be downgraded.");
            Role = newRole;
        }

        public void UpdateWebmail(string webmail)
        {
            if (string.IsNullOrWhiteSpace(webmail))
                throw new ArgumentException("Webmail is required.");

            Webmail = webmail;
        }
    }
}
