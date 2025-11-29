using Microsoft.EntityFrameworkCore;
using SmartAccessLift.Web.Data;
using SmartAccessLift.Web.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add session support
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Configure Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure SignalR
builder.Services.AddSignalR();

// Register application services
builder.Services.AddScoped<SmartAccessLift.Web.Services.IAuthenticationService, SmartAccessLift.Web.Services.AuthenticationService>();
builder.Services.AddScoped<SmartAccessLift.Web.Services.IVisitorAccessService, SmartAccessLift.Web.Services.VisitorAccessService>();
builder.Services.AddScoped<SmartAccessLift.Web.Services.IQRCodeService, SmartAccessLift.Web.Services.QRCodeService>();
builder.Services.AddScoped<SmartAccessLift.Web.Services.IFloorPermissionService, SmartAccessLift.Web.Services.FloorPermissionService>();
builder.Services.AddScoped<SmartAccessLift.Web.Services.IAccessLogService, SmartAccessLift.Web.Services.AccessLogService>();
builder.Services.AddScoped<SmartAccessLift.Web.Services.ICameraService, SmartAccessLift.Web.Services.CameraService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Use session
app.UseSession();

// Authentication and Authorization (will be configured with Identity in later phases)
// app.UseAuthentication();
app.UseAuthorization();

// Map SignalR hub
app.MapHub<ElevatorHub>("/hubs/elevator");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

// Seed database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        await DbSeeder.SeedAsync(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.Run();
