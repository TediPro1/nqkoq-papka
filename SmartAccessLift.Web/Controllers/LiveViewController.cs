using Microsoft.AspNetCore.Mvc;
using SmartAccessLift.Web.Attributes;
using SmartAccessLift.Web.Models.ViewModels;
using SmartAccessLift.Web.Services;

namespace SmartAccessLift.Web.Controllers;

[Authorize]
public class LiveViewController : Controller
{
    private readonly ICameraService _cameraService;

    public LiveViewController(ICameraService cameraService)
    {
        _cameraService = cameraService;
    }

    public IActionResult Index()
    {
        var viewModel = new LiveViewViewModel
        {
            CameraFeedUrl = _cameraService.GetCameraFeedUrl(),
            CurrentOccupants = new List<OccupantViewModel>()
            // TODO: Load actual occupants from real-time data
        };

        return View(viewModel);
    }

    [HttpGet]
    public IActionResult Occupants()
    {
        // TODO: Get actual occupants from real-time tracking
        var occupants = new List<OccupantViewModel>
        {
            // Placeholder data
        };

        return Json(new
        {
            occupants = occupants,
            timestamp = DateTime.UtcNow
        });
    }
}

