using Microsoft.AspNetCore.Mvc;
using SmartAccessLift.Web.Attributes;
using SmartAccessLift.Web.Helpers;
using SmartAccessLift.Web.Models.ViewModels;
using SmartAccessLift.Web.Services;

namespace SmartAccessLift.Web.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly IVisitorAccessService _visitorAccessService;
    private readonly ICameraService _cameraService;

    public DashboardController(IVisitorAccessService visitorAccessService, ICameraService cameraService)
    {
        _visitorAccessService = visitorAccessService;
        _cameraService = cameraService;
    }

    public async Task<IActionResult> Index()
    {
        var userId = SessionHelper.GetUserId(HttpContext.Session) ?? 0;
        var userName = SessionHelper.GetUserName(HttpContext.Session) ?? "User";

        // Load upcoming visitors for admins
        var upcomingVisitors = new List<UpcomingVisitorViewModel>();
        if (SessionHelper.IsAdmin(HttpContext.Session))
        {
            var visitorAccesses = await _visitorAccessService.GetUserVisitorAccessesAsync(userId, null);
            upcomingVisitors = visitorAccesses
                .Where(va => va.Status == "Pending" || va.Status == "Active")
                .OrderBy(va => va.StartTime)
                .Take(10)
                .Select(va => new UpcomingVisitorViewModel
                {
                    Id = va.Id,
                    VisitorName = va.VisitorName,
                    StartTime = va.StartTime,
                    EndTime = va.EndTime,
                    Status = va.Status
                })
                .ToList();
        }

        var viewModel = new DashboardViewModel
        {
            UserName = userName,
            CameraFeedUrl = _cameraService.GetCameraFeedUrl(),
            UpcomingVisitors = upcomingVisitors,
            ActiveVisitorCount = upcomingVisitors.Count(v => v.Status == "Active"),
            PendingVisitorCount = upcomingVisitors.Count(v => v.Status == "Pending")
        };

        return View(viewModel);
    }
}

