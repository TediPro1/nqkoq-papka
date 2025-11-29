using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SmartAccessLift.Web.Models.Entities;

namespace SmartAccessLift.Web.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        // Seed Floors if none exist
        if (!await context.Floors.AnyAsync())
        {
            var floors = new List<Floor>();
            for (int i = 1; i <= 50; i++)
            {
                floors.Add(new Floor
                {
                    FloorNumber = i,
                    Name = i == 1 ? "Lobby" : null,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });
            }
            context.Floors.AddRange(floors);
        }

        // Seed Admin user if none exists
        if (!await context.Users.AnyAsync(u => u.Role == "Admin"))
        {
            var adminPasswordHash = HashPassword("Admin123!"); // Default admin password
            var adminUser = new User
            {
                Email = "admin@smartaccess.com",
                PasswordHash = adminPasswordHash,
                FirstName = "Admin",
                LastName = "User",
                Role = "Admin",
                IsActive = true,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };
            context.Users.Add(adminUser);
        }

        await context.SaveChangesAsync();
    }

    private static string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}

