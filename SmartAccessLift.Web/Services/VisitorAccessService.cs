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

        // Validate user exists
        var userExists = await _context.Users.AnyAsync(u => u.Id == createdByUserId && u.IsActive);
        if (!userExists)
        {
            throw new InvalidOperationException("User not found or inactive");
        }

        // Generate QR code
        var qrToken = _qrCodeService.GenerateQRCodeToken();
        var qrCodeData = $"smartaccess://visitor/{qrToken}";
        var qrCodeImage = _qrCodeService.GenerateQRCodeImage(qrCodeData);

        // Create the full data URL
        var qrCodeImageUrl = $"data:image/png;base64,{qrCodeImage}";
        
        // Note: QRCodeImageUrl field has MaxLength(1000), but base64 images can be longer
        // For now, we'll store a truncated version in the DB but return the full URL in the response
        // In production, consider storing images as files or increasing the field size

        // Store truncated version in DB if too long (for database constraint)
        // But we'll return the full image in the response
        var dbImageUrl = qrCodeImageUrl.Length > 1000 
            ? qrCodeImageUrl.Substring(0, 1000) 
            : qrCodeImageUrl;

        var visitorAccess = new VisitorAccess
        {
            CreatedByUserId = createdByUserId,
            VisitorName = request.VisitorName,
            QRCode = qrToken,
            QRCodeImageUrl = dbImageUrl, // Store truncated version in DB
            StartTime = request.StartTime.ToUniversalTime(),
            EndTime = request.EndTime.ToUniversalTime(),
            Status = DateTime.UtcNow >= request.StartTime.ToUniversalTime() ? "Active" : "Pending",
            CreatedAt = DateTime.UtcNow,
            UseCount = 0
        };

        _context.VisitorAccesses.Add(visitorAccess);
        await _context.SaveChangesAsync();

        // Add floor associations
        if (request.FloorIds != null && request.FloorIds.Any())
        {
            foreach (var floorId in request.FloorIds)
            {
                // Verify floor exists
                var floorExists = await _context.Floors.AnyAsync(f => f.Id == floorId && f.IsActive);
                if (!floorExists)
                {
                    throw new InvalidOperationException($"Floor with ID {floorId} does not exist or is not active");
                }

                _context.VisitorAccessFloors.Add(new VisitorAccessFloor
                {
                    VisitorAccessId = visitorAccess.Id,
                    FloorId = floorId
                });
            }

            await _context.SaveChangesAsync();
        }

        // Update the QRCodeImageUrl with the full image (for response)
        // The DB only stores a truncated version due to field size limit
        visitorAccess.QRCodeImageUrl = qrCodeImageUrl;

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

