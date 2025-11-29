# Quick Start Guide: SmartAccess Lift

**Date**: 2024-12-19  
**Feature**: SmartAccess Lift UI/UX Design  
**Tech Stack**: C# ASP.NET Core MVC, CSHTML, CSS, JS, MSSQL

## Prerequisites

- .NET 8.0 SDK or later
- SQL Server 2019 or later (or SQL Server Express)
- Visual Studio 2022 or Visual Studio Code with C# extension
- Git (for version control)

## Initial Setup

### 1. Create Solution and Projects

```bash
# Create solution
dotnet new sln -n SmartAccessLift

# Create main web application
dotnet new mvc -n SmartAccessLift.Web -o SmartAccessLift.Web
dotnet sln add SmartAccessLift.Web/SmartAccessLift.Web.csproj

# Create test project
dotnet new xunit -n SmartAccessLift.Tests -o SmartAccessLift.Tests
dotnet sln add SmartAccessLift.Tests/SmartAccessLift.Tests.csproj
dotnet add SmartAccessLift.Tests/SmartAccessLift.Tests.csproj reference SmartAccessLift.Web/SmartAccessLift.Web.csproj
```

### 2. Install Required NuGet Packages

```bash
cd SmartAccessLift.Web

# Entity Framework Core and SQL Server provider
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.EntityFrameworkCore.Design

# ASP.NET Core Identity (for authentication)
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore

# SignalR (for real-time updates)
dotnet add package Microsoft.AspNetCore.SignalR

# QR Code generation
dotnet add package QRCoder

# Testing packages (in test project)
cd ../SmartAccessLift.Tests
dotnet add package Moq
dotnet add package FluentAssertions
dotnet add package Microsoft.AspNetCore.Mvc.Testing
```

### 3. Configure Database Connection

Edit `SmartAccessLift.Web/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SmartAccessLift;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### 4. Create ApplicationDbContext

Create `SmartAccessLift.Web/Data/ApplicationDbContext.cs`:

```csharp
using Microsoft.EntityFrameworkCore;
using SmartAccessLift.Web.Models.Entities;

namespace SmartAccessLift.Web.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Floor> Floors { get; set; }
        public DbSet<FloorPermission> FloorPermissions { get; set; }
        public DbSet<VisitorAccess> VisitorAccesses { get; set; }
        public DbSet<VisitorAccessFloor> VisitorAccessFloors { get; set; }
        public DbSet<AccessLog> AccessLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure entities, relationships, and constraints
            // See data-model.md for detailed configuration
        }
    }
}
```

### 5. Configure Services in Program.cs

Edit `SmartAccessLift.Web/Program.cs`:

```csharp
using Microsoft.EntityFrameworkCore;
using SmartAccessLift.Web.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Add Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add SignalR
builder.Services.AddSignalR();

// Register application services
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IVisitorAccessService, VisitorAccessService>();
builder.Services.AddScoped<IFloorPermissionService, FloorPermissionService>();
builder.Services.AddScoped<IAccessLogService, AccessLogService>();
builder.Services.AddScoped<IQRCodeService, QRCodeService>();
builder.Services.AddScoped<ICameraService, CameraService>();

// Add authentication (if using ASP.NET Core Identity)
// builder.Services.AddDefaultIdentity<IdentityUser>(...)

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

// Map SignalR hub
app.MapHub<ElevatorHub>("/hubs/elevator");

app.Run();
```

### 6. Create Initial Migration

```bash
cd SmartAccessLift.Web
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 7. Seed Initial Data

Create a data seeding class or add to `Program.cs`:

```csharp
// Seed admin user and floors
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    // Seed data here
}
```

## Project Structure Setup

### Create Directory Structure

```bash
cd SmartAccessLift.Web

# Create directories
mkdir -p Controllers Models/ViewModels Models/Entities Services Data Hubs Views/Account Views/Dashboard Views/VisitorAccess Views/FloorPermission Views/LiveView Views/AccessLog Views/Shared wwwroot/css wwwroot/js wwwroot/images
```

## Development Workflow

### 1. Create Entity Models

Create entity classes in `Models/Entities/` based on `data-model.md`:
- `User.cs`
- `Floor.cs`
- `FloorPermission.cs`
- `VisitorAccess.cs`
- `VisitorAccessFloor.cs`
- `AccessLog.cs`

### 2. Create View Models

Create view model classes in `Models/ViewModels/` based on API contracts:
- `LoginViewModel.cs`
- `RegisterViewModel.cs`
- `DashboardViewModel.cs`
- `VisitorAccessViewModel.cs`
- `FloorPermissionViewModel.cs`
- `LiveViewViewModel.cs`
- `AccessLogViewModel.cs`

### 3. Create Services

Create service interfaces and implementations in `Services/`:
- `IAuthenticationService.cs` / `AuthenticationService.cs`
- `IVisitorAccessService.cs` / `VisitorAccessService.cs`
- `IFloorPermissionService.cs` / `FloorPermissionService.cs`
- `IAccessLogService.cs` / `AccessLogService.cs`
- `IQRCodeService.cs` / `QRCodeService.cs`
- `ICameraService.cs` / `CameraService.cs`

### 4. Create Controllers

Create controllers in `Controllers/`:
- `AccountController.cs` - Login, Register, Logout
- `DashboardController.cs` - Home dashboard
- `VisitorAccessController.cs` - Visitor access management
- `FloorPermissionController.cs` - Floor permission management
- `LiveViewController.cs` - Live elevator view
- `AccessLogController.cs` - Access logs

### 5. Create Views (CSHTML)

Create Razor views in `Views/`:
- `Views/Account/Login.cshtml`
- `Views/Account/Register.cshtml`
- `Views/Dashboard/Index.cshtml`
- `Views/VisitorAccess/Index.cshtml`
- `Views/FloorPermission/Index.cshtml`
- `Views/LiveView/Index.cshtml`
- `Views/AccessLog/Index.cshtml`
- `Views/Shared/_Layout.cshtml`
- `Views/Shared/_Navigation.cshtml`

### 6. Create CSS

Create stylesheets in `wwwroot/css/`:
- `site.css` - Main stylesheet with color palette (CSS variables)
- `dashboard.css` - Dashboard-specific styles
- `components.css` - Reusable component styles (buttons, cards, modals)

### 7. Create JavaScript

Create JavaScript files in `wwwroot/js/`:
- `site.js` - Main JavaScript
- `dashboard.js` - Dashboard interactivity
- `visitor-access.js` - Visitor access form handling
- `floor-permission.js` - Floor permission toggles
- `live-view.js` - Live view video handling
- `access-log.js` - Access log filtering/pagination
- `signalr.js` - SignalR client connection

### 8. Create SignalR Hub

Create `Hubs/ElevatorHub.cs`:

```csharp
using Microsoft.AspNetCore.SignalR;

namespace SmartAccessLift.Web.Hubs
{
    public class ElevatorHub : Hub
    {
        public async Task JoinElevatorGroup()
        {
            // Add user to elevator monitoring group based on permissions
            await Groups.AddToGroupAsync(Context.ConnectionId, "elevator-monitors");
        }

        // Server methods to call from services
        public async Task NotifyOccupantEntered(object occupantData)
        {
            await Clients.Group("elevator-monitors").SendAsync("OccupantEntered", occupantData);
        }
    }
}
```

## Running the Application

### Development Mode

```bash
cd SmartAccessLift.Web
dotnet run
```

Application will be available at `https://localhost:5001` or `http://localhost:5000`

### Production Build

```bash
dotnet publish -c Release -o ./publish
```

## Testing

### Run Unit Tests

```bash
cd SmartAccessLift.Tests
dotnet test
```

### Run with Coverage

```bash
dotnet test /p:CollectCoverage=true
```

## Key Implementation Notes

### Color Palette (CSS Variables)

Add to `wwwroot/css/site.css`:

```css
:root {
  --primary: #4F46E5;
  --primary-alt: #6366F1;
  --accent-green: #22C55E;
  --accent-orange: #F59E0B;
  --accent-red: #EF4444;
  --bg-light: #F3F4F6;
  --bg-white: #FFFFFF;
  --text-dark: #1F2937;
  --border-radius: 20px;
  --shadow-soft: 0 4px 6px rgba(0, 0, 0, 0.1);
}
```

### QR Code Generation Example

```csharp
using QRCoder;

public class QRCodeService : IQRCodeService
{
    public string GenerateQRCodeImage(string data)
    {
        using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
        {
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
            using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
            {
                byte[] qrCodeBytes = qrCode.GetGraphic(20);
                return Convert.ToBase64String(qrCodeBytes);
            }
        }
    }
}
```

### SignalR Client Connection

Add to `wwwroot/js/signalr.js`:

```javascript
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/elevator")
    .build();

connection.start().then(() => {
    console.log("SignalR Connected");
    connection.invoke("JoinElevatorGroup");
});

connection.on("OccupantEntered", (data) => {
    // Update UI with new occupant
    updateOccupantsList(data);
});
```

## Next Steps

1. Implement authentication (ASP.NET Core Identity or custom)
2. Create database migrations and seed data
3. Implement controllers and services
4. Build CSHTML views with modern CSS
5. Add JavaScript for interactivity
6. Implement SignalR for real-time updates
7. Add QR code generation
8. Test all user flows
9. Deploy to staging environment

## Troubleshooting

### Database Connection Issues

- Verify SQL Server is running
- Check connection string in `appsettings.json`
- Ensure database exists or use `dotnet ef database update`

### SignalR Connection Issues

- Verify SignalR hub is mapped in `Program.cs`
- Check CORS settings if accessing from different origin
- Verify JavaScript SignalR client library is loaded

### Migration Issues

- Delete `Migrations/` folder and recreate if schema changes are complex
- Use `dotnet ef migrations remove` to remove last migration
- Use `dotnet ef database drop` to reset database (development only)

## Resources

- [ASP.NET Core MVC Documentation](https://docs.microsoft.com/aspnet/core/mvc/)
- [Entity Framework Core Documentation](https://docs.microsoft.com/ef/core/)
- [SignalR Documentation](https://docs.microsoft.com/aspnet/core/signalr/)
- [QRCoder GitHub](https://github.com/codebude/QRCoder)

