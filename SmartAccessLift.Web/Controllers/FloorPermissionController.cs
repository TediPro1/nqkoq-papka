using Microsoft.AspNetCore.Mvc;
using SmartAccessLift.Web.Models.ViewModels;
using SmartAccessLift.Web.Services;

namespace SmartAccessLift.Web.Controllers;

public class FloorPermissionController : Controller
{
    private readonly IFloorPermissionService _floorPermissionService;

    public FloorPermissionController(IFloorPermissionService floorPermissionService)
    {
        _floorPermissionService = floorPermissionService;
    }

    public async Task<IActionResult> Index(int? userId = null)
    {
        // TODO: Get current user ID from authentication
        int currentUserId = userId ?? 1; // Placeholder

        var floors = await _floorPermissionService.GetUserFloorPermissionsAsync(currentUserId);

        var viewModel = new FloorPermissionViewModel
        {
            Floors = floors,
            IsAdmin = false, // TODO: Check if user is admin
            TargetUserId = userId
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
            // TODO: Get current user ID from authentication
            int currentUserId = request.UserId ?? 1; // Placeholder
            int? grantedBy = null; // TODO: Set if admin is granting

            var servicePermissions = request.Permissions.Select(p => new Services.FloorPermissionUpdate
            {
                FloorId = p.FloorId,
                IsAllowed = p.IsAllowed
            }).ToList();

            await _floorPermissionService.UpdateFloorPermissionsAsync(
                currentUserId,
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

