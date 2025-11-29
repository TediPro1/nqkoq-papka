using Microsoft.AspNetCore.Mvc;
using SmartAccessLift.Web.Models.ViewModels;

namespace SmartAccessLift.Web.Controllers;

public class DashboardController : Controller
{
    public IActionResult Index()
    {
        var viewModel = new DashboardViewModel
        {
            UserName = "Resident", // TODO: Get from authenticated user
            CameraFeedUrl = "/api/camera/feed", // TODO: Get actual camera feed URL
            UpcomingVisitors = new List<UpcomingVisitorViewModel>
            {
                // TODO: Load from database
            },
            ActiveVisitorCount = 0,
            PendingVisitorCount = 0
        };

        return View(viewModel);
    }
}

