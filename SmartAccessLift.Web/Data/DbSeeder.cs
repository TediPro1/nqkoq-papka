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
            // Note: In production, use proper password hashing (ASP.NET Core Identity)
            var adminUser = new User
            {
                Email = "admin@smartaccess.com",
                PasswordHash = "PLACEHOLDER_HASH", // Will be replaced with proper Identity hashing
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
}

