using Microsoft.EntityFrameworkCore;
using SmartAccessLift.Web.Data;
using SmartAccessLift.Web.Models.ViewModels;

namespace SmartAccessLift.Web.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly ApplicationDbContext _context;

    public AuthenticationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> AuthenticateAsync(string email, string password)
    {
        // TODO: Implement proper password hashing and verification
        // For now, simple placeholder
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);

        if (user == null)
            return false;

        // TODO: Verify password hash
        // For now, just check if user exists
        return true;
    }

    public async Task<bool> RegisterAsync(RegisterViewModel model)
    {
        // TODO: Implement proper registration with password hashing
        // Check if email already exists
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == model.Email);

        if (existingUser != null)
            return false;

        // TODO: Hash password properly
        var user = new Models.Entities.User
        {
            Email = model.Email,
            PasswordHash = "PLACEHOLDER_HASH", // TODO: Hash password
            FirstName = model.FirstName,
            LastName = model.LastName,
            Role = "Resident",
            IsActive = true,
            EmailConfirmed = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return true;
    }

    public Task SignOutAsync()
    {
        // TODO: Implement sign out logic
        return Task.CompletedTask;
    }
}

