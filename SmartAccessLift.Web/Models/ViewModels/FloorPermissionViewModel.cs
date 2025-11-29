namespace SmartAccessLift.Web.Models.ViewModels;

public class FloorPermissionViewModel
{
    public List<FloorPermissionItemViewModel> Floors { get; set; } = new List<FloorPermissionItemViewModel>();
    public bool IsAdmin { get; set; }
    public int? TargetUserId { get; set; }
}

public class FloorPermissionItemViewModel
{
    public int FloorId { get; set; }
    public int FloorNumber { get; set; }
    public string? FloorName { get; set; }
    public bool IsAllowed { get; set; }
    public bool CanModify { get; set; }
}

public class UpdateFloorPermissionRequest
{
    public int? UserId { get; set; }
    public List<FloorPermissionUpdate> Permissions { get; set; } = new List<FloorPermissionUpdate>();
}

public class FloorPermissionUpdate
{
    public int FloorId { get; set; }
    public bool IsAllowed { get; set; }
}

