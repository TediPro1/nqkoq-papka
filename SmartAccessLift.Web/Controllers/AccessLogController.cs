using Microsoft.AspNetCore.Mvc;
using SmartAccessLift.Web.Attributes;
using SmartAccessLift.Web.Helpers;
using SmartAccessLift.Web.Models.ViewModels;
using SmartAccessLift.Web.Services;

namespace SmartAccessLift.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AccessLogController : Controller
{
    private readonly IAccessLogService _accessLogService;

    public AccessLogController(IAccessLogService accessLogService)
    {
        _accessLogService = accessLogService;
    }

    public IActionResult Index()
    {
        var viewModel = new AccessLogViewModel
        {
            Filter = new AccessLogFilterViewModel()
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Data(int? floorId, string? accessMethod, string? outcome, DateTime? startDate, DateTime? endDate, int? userId, int page = 1, int pageSize = 50)
    {
        var currentUserId = SessionHelper.GetUserId(HttpContext.Session) ?? 0;
        var isAdmin = SessionHelper.IsAdmin(HttpContext.Session);

        // If not admin, restrict to user's accessible floors
        if (!isAdmin)
        {
            // TODO: Filter by user's accessible floors
        }

        var logs = await _accessLogService.GetAccessLogsAsync(
            userId ?? (isAdmin ? null : currentUserId),
            floorId,
            accessMethod,
            outcome,
            startDate,
            endDate,
            page,
            pageSize);

        var totalCount = await _accessLogService.GetAccessLogCountAsync(
            userId ?? (isAdmin ? null : currentUserId),
            floorId,
            accessMethod,
            outcome,
            startDate,
            endDate);

        var logViewModels = logs.Select(log => new AccessLogEntryViewModel
        {
            Id = log.Id,
            UserName = log.User?.Email,
            VisitorName = log.VisitorAccess?.VisitorName,
            FloorNumber = log.Floor.FloorNumber,
            AccessMethod = log.AccessMethod,
            Timestamp = log.Timestamp,
            Outcome = log.Outcome,
            Reason = log.Reason
        }).ToList();

        return Json(new
        {
            logs = logViewModels,
            totalCount = totalCount,
            page = page,
            pageSize = pageSize,
            totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        });
    }
}

