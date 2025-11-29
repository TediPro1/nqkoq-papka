using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartAccessLift.Web.Attributes;
using SmartAccessLift.Web.Data;
using SmartAccessLift.Web.Helpers;
using SmartAccessLift.Web.Models.ViewModels;
using SmartAccessLift.Web.Services;

namespace SmartAccessLift.Web.Controllers;

[Authorize(Roles = "Admin")]
public class FloorPermissionController : Controller
{
    private readonly IFloorPermissionService _floorPermissionService;
    private readonly ApplicationDbContext _context;

    public FloorPermissionController(IFloorPermissionService floorPermissionService, ApplicationDbContext context)
    {
        _floorPermissionService = floorPermissionService;
        _context = context;
    }

    public async Task<IActionResult> Index(int? userId = null)
    {
        var isAdmin = SessionHelper.IsAdmin(HttpContext.Session);
        var targetUserId = userId ?? SessionHelper.GetUserId(HttpContext.Session) ?? 0;

        // Get all active users for admin to select from
        var allUsers = await _context.Users
            .Where(u => u.IsActive)
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .Select(u => new UserOptionViewModel
            {
                UserId = u.Id,
                FullName = $"{u.FirstName} {u.LastName}",
                Email = u.Email,
                Role = u.Role
            })
            .ToListAsync();

        var floors = await _floorPermissionService.GetUserFloorPermissionsAsync(targetUserId);

        // Get selected user info
        var selectedUser = allUsers.FirstOrDefault(u => u.UserId == targetUserId);

        var viewModel = new FloorPermissionViewModel
        {
            Floors = floors,
            IsAdmin = isAdmin,
            TargetUserId = targetUserId,
            AllUsers = allUsers,
            SelectedUserName = selectedUser?.FullName ?? "Unknown User"
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update([FromBody] UpdateFloorPermissionRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var targetUserId = request.UserId ?? SessionHelper.GetUserId(HttpContext.Session) ?? 0;
            var isAdmin = SessionHelper.IsAdmin(HttpContext.Session);
            
            // Validate that the target user exists
            var targetUser = await _context.Users.FindAsync(targetUserId);
            if (targetUser == null || !targetUser.IsActive)
            {
                return BadRequest(new { success = false, errors = new[] { "Invalid user selected" } });
            }

            int? grantedBy = isAdmin ? SessionHelper.GetUserId(HttpContext.Session) : null;

            var servicePermissions = request.Permissions.Select(p => new Services.FloorPermissionUpdate
            {
                FloorId = p.FloorId,
                IsAllowed = p.IsAllowed
            }).ToList();

            await _floorPermissionService.UpdateFloorPermissionsAsync(
                targetUserId,
                servicePermissions,
                grantedBy);

            return Json(new { success = true, message = "Permissions updated successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, errors = new[] { ex.Message } });
        }
    }
}

