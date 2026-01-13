using IskoAlert_WebApp.Models.Domain;
using IskoAlert_WebApp.Models.Domain.Enums;
using IskoAlert_WebApp.Models.ViewModels.Account;

namespace IskoAlert_WebApp.Services.Interfaces
{
    public interface IUserService
    {
        Task RegisterAsync(RegisterViewModel model);
        Task<User?> ValidateCredentialsAsync(string email, string password, UserRole role);
    }


}