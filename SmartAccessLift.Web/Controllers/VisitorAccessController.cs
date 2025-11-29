using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartAccessLift.Web.Attributes;
using SmartAccessLift.Web.Data;
using SmartAccessLift.Web.Helpers;
using SmartAccessLift.Web.Models.ViewModels;
using SmartAccessLift.Web.Services;

namespace SmartAccessLift.Web.Controllers;

[Authorize]
public class VisitorAccessController : Controller
{
    private readonly IVisitorAccessService _visitorAccessService;
    private readonly ApplicationDbContext _context;
    private readonly IQRCodeService _qrCodeService;

    public VisitorAccessController(IVisitorAccessService visitorAccessService, ApplicationDbContext context, IQRCodeService qrCodeService)
    {
        _visitorAccessService = visitorAccessService;
        _context = context;
        _qrCodeService = qrCodeService;
    }

    // In VisitorAccessController.cs, update the Index action
    public async Task<IActionResult> Index()
    {
        var currentUserId = SessionHelper.GetUserId(HttpContext.Session) ?? 0;
        if (currentUserId == 0)
        {
            return RedirectToAction("Login", "Account");
        }

        // Get the floors that the current user has permission to grant access to
        // Only include floors that are active and the user has permission for
        var floorPermissions = await _context.FloorPermissions
            .Where(fp => fp.UserId == currentUserId && fp.IsAllowed)
            .Include(fp => fp.Floor)
            .ToListAsync();

        var userPermissions = floorPermissions
            .Where(fp => fp.Floor != null && fp.Floor.IsActive)
            .Select(fp => new FloorOptionViewModel
            {
                FloorId = fp.FloorId,
                FloorNumber = fp.Floor.FloorNumber,
                FloorName = fp.Floor.Name,
                IsAllowed = fp.IsAllowed
            })
            .OrderBy(f => f.FloorNumber)
            .ToList();

        var viewModel = new VisitorAccessViewModel
        {
            AvailableFloors = userPermissions
        };

        return View(viewModel);
    }

    [HttpPost]
    // Note: Anti-forgery token validation is disabled for JSON requests
    // The endpoint is still protected by [Authorize] attribute
    public async Task<IActionResult> Create([FromBody] CreateVisitorAccessRequest request)
    {
        if (request == null)
        {
            return BadRequest(new { success = false, error = "Request body is required" });
        }

        // Validate dates are in the future or at least EndTime is after StartTime
        if (request.EndTime <= request.StartTime)
        {
            return BadRequest(new { success = false, error = "End time must be after start time" });
        }

        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .SelectMany(x => x.Value.Errors.Select(e => $"{x.Key}: {e.ErrorMessage}"))
                .ToList();
            return BadRequest(new { success = false, error = string.Join(", ", errors), errors = errors });
        }

        try
        {
            var currentUserId = SessionHelper.GetUserId(HttpContext.Session) ?? 0;
            if (currentUserId == 0)
            {
                return BadRequest(new { success = false, error = "User not authenticated" });
            }

            var visitorAccess = await _visitorAccessService.CreateVisitorAccessAsync(request, currentUserId);

            var shareableLink = $"{Request.Scheme}://{Request.Host}/access/{visitorAccess.QRCode}";

            // Generate the full QR code image URL for the response
            // (The DB may have a truncated version due to field size limits)
            var qrCodeData = $"smartaccess://visitor/{visitorAccess.QRCode}";
            var qrCodeImageBase64 = _qrCodeService.GenerateQRCodeImage(qrCodeData);
            var fullQrCodeImageUrl = $"data:image/png;base64,{qrCodeImageBase64}";

            // Generate a random access code (6 digits)
            var random = new Random();
            var accessCode = random.Next(100000, 999999).ToString();

            return Json(new
            {
                success = true,
                visitorAccessId = visitorAccess.Id,
                qrCodeUrl = Url.Action("QRCode", new { id = visitorAccess.Id }),
                qrCodeImage = fullQrCodeImageUrl,
                shareableLink = shareableLink,
                accessCode = accessCode
            });
        }
        catch (Exception ex)
        {
            // Get the inner exception message for more details
            var errorMessage = ex.Message;
            if (ex.InnerException != null)
            {
                errorMessage += $" Inner: {ex.InnerException.Message}";
            }
            return BadRequest(new { success = false, error = errorMessage });
        }
    }

    public async Task<IActionResult> QRCode(int id)
    {
        var visitorAccess = await _visitorAccessService.GetVisitorAccessByIdAsync(id);
        if (visitorAccess == null)
        {
            return NotFound();
        }

        var shareableLink = $"{Request.Scheme}://{Request.Host}/access/{visitorAccess.QRCode}";

        var viewModel = new QRCodeViewModel
        {
            VisitorAccessId = visitorAccess.Id,
            VisitorName = visitorAccess.VisitorName,
            QRCodeImageUrl = visitorAccess.QRCodeImageUrl ?? string.Empty,
            ShareableLink = shareableLink,
            ExpiresAt = visitorAccess.EndTime
        };

        return PartialView(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> List(string? status = null, int page = 1, int pageSize = 10)
    {
        var currentUserId = SessionHelper.GetUserId(HttpContext.Session) ?? 0;

        var visitorAccesses = await _visitorAccessService.GetUserVisitorAccessesAsync(currentUserId, status);

        var totalCount = visitorAccesses.Count;
        var items = visitorAccesses
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(va => new
            {
                id = va.Id,
                visitorName = va.VisitorName,
                startTime = va.StartTime,
                endTime = va.EndTime,
                status = va.Status,
                floors = va.VisitorAccessFloors.Select(vaf => vaf.FloorId).ToList(),
                useCount = va.UseCount
            })
            .ToList();

        return Json(new
        {
            items = items,
            totalCount = totalCount,
            page = page,
            pageSize = pageSize,
            totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        });
    }
}

