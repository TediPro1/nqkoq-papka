using SmartAccessLift.Web.Models.Entities;
using SmartAccessLift.Web.Models.ViewModels;

namespace SmartAccessLift.Web.Services;

public interface IAuthenticationService
{
    Task<User?> AuthenticateAsync(string email, string password);
    Task<User?> RegisterAsync(RegisterViewModel model);
    Task SignOutAsync();
}

