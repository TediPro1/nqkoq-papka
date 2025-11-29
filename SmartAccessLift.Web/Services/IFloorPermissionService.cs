using SmartAccessLift.Web.Models.Entities;
using SmartAccessLift.Web.Models.ViewModels;

namespace SmartAccessLift.Web.Services;

public interface IFloorPermissionService
{
    Task<List<FloorPermissionItemViewModel>> GetUserFloorPermissionsAsync(int userId);
    Task UpdateFloorPermissionsAsync(int userId, List<FloorPermissionUpdate> permissions, int? grantedBy = null);
    Task<bool> CanUserModifyPermissionAsync(int userId, int floorId);
}

public class FloorPermissionUpdate
{
    public int FloorId { get; set; }
    public bool IsAllowed { get; set; }
}

