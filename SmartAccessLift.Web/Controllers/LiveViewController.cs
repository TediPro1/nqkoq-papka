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
            CurrentOccupants = new List<OccupantViewModel>
            {
                new OccupantViewModel
                {
                    VisitorName = "Леля Гинка",
                    EntryTime = new DateTime(2023, 3, 15, 16, 36, 0),
                    CurrentFloor = 3,
                    AccessMethod = "QR"
                }
            },
            CurrentFloor = 1, // TODO: Get from real-time data
            DoorStatus = "Closed" // TODO: Get from real-time data
        };

        return View(viewModel);
    }

    [HttpGet]
    public IActionResult Occupants()
    {
        // TODO: Get actual occupants from real-time tracking
        var occupants = new List<OccupantViewModel>
        {
            new OccupantViewModel
            {
                VisitorName = "Леля Гинка",
                EntryTime = new DateTime(2023, 3, 15, 16, 36, 0),
                CurrentFloor = 3,
                AccessMethod = "QR"
            }
        };

        return Json(new
        {
            occupants = occupants,
            timestamp = DateTime.UtcNow
        });
    }
}

