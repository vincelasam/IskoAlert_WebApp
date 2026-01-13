using IskoAlert_WebApp.Models.ViewModels.Account;

namespace IskoAlert_WebApp.Services.Interfaces
{
    public interface IUserService
    {
        Task RegisterAsync(RegisterViewModel model);
    }
}