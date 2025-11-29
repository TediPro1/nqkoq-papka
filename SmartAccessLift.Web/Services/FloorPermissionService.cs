using Microsoft.EntityFrameworkCore;
using SmartAccessLift.Web.Data;
using SmartAccessLift.Web.Models.Entities;
using SmartAccessLift.Web.Models.ViewModels;

namespace SmartAccessLift.Web.Services;

public class FloorPermissionService : IFloorPermissionService
{
    private readonly ApplicationDbContext _context;

    public FloorPermissionService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<FloorPermissionItemViewModel>> GetUserFloorPermissionsAsync(int userId)
    {
        var floors = await _context.Floors
            .Where(f => f.IsActive)
            .OrderBy(f => f.FloorNumber)
            .ToListAsync();

        var permissions = await _context.FloorPermissions
            .Where(fp => fp.UserId == userId)
            .ToListAsync();

        return floors.Select(floor =>
        {
            var permission = permissions.FirstOrDefault(p => p.FloorId == floor.Id);
            return new FloorPermissionItemViewModel
            {
                FloorId = floor.Id,
                FloorNumber = floor.FloorNumber,
                FloorName = floor.Name,
                IsAllowed = permission?.IsAllowed ?? false,
                CanModify = true // TODO: Check if user can modify this permission
            };
        }).ToList();
    }

    public async Task UpdateFloorPermissionsAsync(int userId, List<FloorPermissionUpdate> permissions, int? grantedBy = null)
    {
        foreach (var permUpdate in permissions)
        {
            var existing = await _context.FloorPermissions
                .FirstOrDefaultAsync(fp => fp.UserId == userId && fp.FloorId == permUpdate.FloorId);

            if (existing != null)
            {
                existing.IsAllowed = permUpdate.IsAllowed;
                existing.GrantedBy = grantedBy;
                existing.GrantedAt = DateTime.UtcNow;
            }
            else
            {
                _context.FloorPermissions.Add(new FloorPermission
                {
                    UserId = userId,
                    FloorId = permUpdate.FloorId,
                    IsAllowed = permUpdate.IsAllowed,
                    GrantedBy = grantedBy,
                    GrantedAt = DateTime.UtcNow
                });
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task<bool> CanUserModifyPermissionAsync(int userId, int floorId)
    {
        // TODO: Implement business logic for permission modification
        // For now, allow modification
        return true;
    }
}

