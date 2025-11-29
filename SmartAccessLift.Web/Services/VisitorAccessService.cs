using Microsoft.EntityFrameworkCore;
using SmartAccessLift.Web.Data;
using SmartAccessLift.Web.Models.Entities;
using SmartAccessLift.Web.Models.ViewModels;

namespace SmartAccessLift.Web.Services;

public class VisitorAccessService : IVisitorAccessService
{
    private readonly ApplicationDbContext _context;
    private readonly IQRCodeService _qrCodeService;

    public VisitorAccessService(ApplicationDbContext context, IQRCodeService qrCodeService)
    {
        _context = context;
        _qrCodeService = qrCodeService;
    }

    public async Task<VisitorAccess> CreateVisitorAccessAsync(CreateVisitorAccessRequest request, int createdByUserId)
    {
        // Validate that user can grant access to requested floors
        var userPermissions = await _context.FloorPermissions
            .Where(fp => fp.UserId == createdByUserId && fp.IsAllowed)
            .Select(fp => fp.FloorId)
            .ToListAsync();

        var invalidFloors = request.FloorIds.Except(userPermissions).ToList();
        if (invalidFloors.Any())
        {
            throw new InvalidOperationException($"You do not have permission to grant access to floors: {string.Join(", ", invalidFloors)}");
        }

        // Generate QR code
        var qrToken = _qrCodeService.GenerateQRCodeToken();
        var qrCodeData = $"smartaccess://visitor/{qrToken}";
        var qrCodeImage = _qrCodeService.GenerateQRCodeImage(qrCodeData);

        var visitorAccess = new VisitorAccess
        {
            CreatedByUserId = createdByUserId,
            VisitorName = request.VisitorName,
            QRCode = qrToken,
            QRCodeImageUrl = $"data:image/png;base64,{qrCodeImage}",
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            Status = DateTime.UtcNow >= request.StartTime ? "Active" : "Pending",
            CreatedAt = DateTime.UtcNow,
            UseCount = 0
        };

        _context.VisitorAccesses.Add(visitorAccess);
        await _context.SaveChangesAsync();

        // Add floor associations
        foreach (var floorId in request.FloorIds)
        {
            _context.VisitorAccessFloors.Add(new VisitorAccessFloor
            {
                VisitorAccessId = visitorAccess.Id,
                FloorId = floorId
            });
        }

        await _context.SaveChangesAsync();

        return visitorAccess;
    }

    public async Task<List<VisitorAccess>> GetUserVisitorAccessesAsync(int userId, string? status = null)
    {
        var query = _context.VisitorAccesses
            .Where(va => va.CreatedByUserId == userId);

        if (!string.IsNullOrEmpty(status) && status != "All")
        {
            query = query.Where(va => va.Status == status);
        }

        return await query
            .OrderByDescending(va => va.CreatedAt)
            .ToListAsync();
    }

    public async Task<VisitorAccess?> GetVisitorAccessByIdAsync(int id)
    {
        return await _context.VisitorAccesses
            .Include(va => va.VisitorAccessFloors)
            .ThenInclude(vaf => vaf.Floor)
            .FirstOrDefaultAsync(va => va.Id == id);
    }

    public async Task UpdateVisitorAccessStatusAsync()
    {
        var now = DateTime.UtcNow;
        var pendingToActive = await _context.VisitorAccesses
            .Where(va => va.Status == "Pending" && va.StartTime <= now && va.EndTime > now)
            .ToListAsync();

        var activeToExpired = await _context.VisitorAccesses
            .Where(va => va.Status == "Active" && va.EndTime <= now)
            .ToListAsync();

        foreach (var va in pendingToActive)
        {
            va.Status = "Active";
        }

        foreach (var va in activeToExpired)
        {
            va.Status = "Expired";
        }

        await _context.SaveChangesAsync();
    }
}

