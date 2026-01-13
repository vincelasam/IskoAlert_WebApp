using IskoAlert_WebApp.Services.Interfaces;
using IskoAlert_WebApp.Models.Domain;
using IskoAlert_WebApp.Models.ViewModels.Account;
using Microsoft.EntityFrameworkCore;
using IskoAlert_WebApp.Data;
using IskoAlert_WebApp.Models.Domain.Enums;
using BCrypt.Net;

namespace IskoAlert_WebApp.Services.Implementations{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        
        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }
    
        public async Task RegisterAsync(RegisterViewModel model)
        {
            //  Business Logic: Check if user already exists
            if (await _context.Users.AnyAsync(u => u.Webmail == model.Webmail))
            {
                throw new InvalidOperationException("Email already registered.");
            }
            
            if (await _context.Users.AnyAsync(u => u.IdNumber == model.IdNumber))
            {
                throw new InvalidOperationException("Student ID already registered.");
            }
            
            //  Business Logic: Hash password
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
            
            //  Transform ViewModel → Domain Model
            var newUser = new User(
                idNumber: model.IdNumber,
                webmail: model.Webmail,
                passwordHash: passwordHash, 
                name: model.FullName,
                role: UserRole.Student
            );
            
            //  Database operations
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
        }
    }
}
