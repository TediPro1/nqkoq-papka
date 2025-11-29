using Microsoft.AspNetCore.Mvc;
using SmartAccessLift.Web.Attributes;
using SmartAccessLift.Web.Helpers;
using SmartAccessLift.Web.Models.ViewModels;
using SmartAccessLift.Web.Services;

namespace SmartAccessLift.Web.Controllers;

[Authorize(Roles = "Admin")]
public class FloorPermissionController : Controller
{
    private readonly IFloorPermissionService _floorPermissionService;

    public FloorPermissionController(IFloorPermissionService floorPermissionService)
    {
        _floorPermissionService = floorPermissionService;
    }

    public async Task<IActionResult> Index(int? userId = null)
    {
        var currentUserId = userId ?? SessionHelper.GetUserId(HttpContext.Session) ?? 0;
        var isAdmin = SessionHelper.IsAdmin(HttpContext.Session);

        var floors = await _floorPermissionService.GetUserFloorPermissionsAsync(currentUserId);

        var viewModel = new FloorPermissionViewModel
        {
            Floors = floors,
            IsAdmin = isAdmin,
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
            var currentUserId = request.UserId ?? SessionHelper.GetUserId(HttpContext.Session) ?? 0;
            var isAdmin = SessionHelper.IsAdmin(HttpContext.Session);
            int? grantedBy = isAdmin ? SessionHelper.GetUserId(HttpContext.Session) : null;

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

