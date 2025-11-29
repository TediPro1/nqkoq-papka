using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SmartAccessLift.Web.Data;
using SmartAccessLift.Web.Models.Entities;
using SmartAccessLift.Web.Models.ViewModels;

namespace SmartAccessLift.Web.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly ApplicationDbContext _context;

    public AuthenticationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> AuthenticateAsync(string email, string password)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);

        if (user == null)
            return null;

        // Verify password hash
        if (!VerifyPassword(password, user.PasswordHash))
            return null;

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<User?> RegisterAsync(RegisterViewModel model)
    {
        // Check if email already exists
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == model.Email);

        if (existingUser != null)
            return null;

        // Hash password
        var passwordHash = HashPassword(model.Password);

        var user = new User
        {
            Email = model.Email,
            PasswordHash = passwordHash,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Role = "Resident", // All new registrations are Residents
            IsActive = true,
            EmailConfirmed = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public Task SignOutAsync()
    {
        return Task.CompletedTask;
    }

    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }

    private bool VerifyPassword(string password, string hash)
    {
        var passwordHash = HashPassword(password);
        return passwordHash == hash;
    }
}

