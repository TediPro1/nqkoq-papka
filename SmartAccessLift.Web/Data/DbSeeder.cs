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
        User? adminUser = null;
        if (!await context.Users.AnyAsync(u => u.Role == "Admin"))
        {
            var adminPasswordHash = HashPassword("Admin123!"); // Default admin password
            adminUser = new User
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

        // Get or create admin user for example logs
        if (adminUser == null)
        {
            adminUser = await context.Users.FirstOrDefaultAsync(u => u.Role == "Admin");
        }

        // Seed example access logs if none exist
        if (!await context.AccessLogs.AnyAsync() && adminUser != null)
        {
            var floors = await context.Floors.Where(f => f.IsActive).Take(10).ToListAsync();
            if (floors.Any())
            {
                var exampleLogs = new List<AccessLog>();
                var random = new Random();
                var accessMethods = new[] { "NFC", "Fingerprint", "QR", "AdminOverride" };
                var outcomes = new[] { "Successful", "Denied" };
                var reasons = new[] { 
                    (string?)null, 
                    "Access granted", 
                    "Invalid credentials", 
                    "Floor access denied",
                    "Visitor access expired"
                };

                // Create example logs for the past 7 days
                for (int i = 0; i < 50; i++)
                {
                    var floor = floors[random.Next(floors.Count)];
                    var accessMethod = accessMethods[random.Next(accessMethods.Length)];
                    var outcome = outcomes[random.Next(outcomes.Length)];
                    var reason = reasons[random.Next(reasons.Length)];
                    var timestamp = DateTime.UtcNow.AddDays(-random.Next(7)).AddHours(-random.Next(24)).AddMinutes(-random.Next(60));

                    exampleLogs.Add(new AccessLog
                    {
                        UserId = adminUser.Id,
                        FloorId = floor.Id,
                        AccessMethod = accessMethod,
                        Outcome = outcome,
                        Timestamp = timestamp,
                        Reason = reason,
                        IPAddress = $"192.168.1.{random.Next(1, 255)}"
                    });
                }

                // Add some visitor access logs
                var visitorAccesses = await context.VisitorAccesses.Take(5).ToListAsync();
                foreach (var visitorAccess in visitorAccesses)
                {
                    var floor = floors[random.Next(floors.Count)];
                    var timestamp = DateTime.UtcNow.AddDays(-random.Next(3)).AddHours(-random.Next(12));

                    exampleLogs.Add(new AccessLog
                    {
                        VisitorAccessId = visitorAccess.Id,
                        FloorId = floor.Id,
                        AccessMethod = "QR",
                        Outcome = "Successful",
                        Timestamp = timestamp,
                        Reason = "QR code validated",
                        IPAddress = $"10.0.0.{random.Next(1, 255)}"
                    });
                }

                context.AccessLogs.AddRange(exampleLogs);
                await context.SaveChangesAsync();
            }
        }
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

