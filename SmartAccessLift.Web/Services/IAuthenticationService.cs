using SmartAccessLift.Web.Models.ViewModels;

namespace SmartAccessLift.Web.Services;

public interface IAuthenticationService
{
    Task<bool> AuthenticateAsync(string email, string password);
    Task<bool> RegisterAsync(RegisterViewModel model);
    Task SignOutAsync();
}

