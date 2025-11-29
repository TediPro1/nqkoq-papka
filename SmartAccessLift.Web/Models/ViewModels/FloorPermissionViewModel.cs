namespace SmartAccessLift.Web.Models.ViewModels;

public class FloorPermissionViewModel
{
    public List<FloorPermissionItemViewModel> Floors { get; set; } = new List<FloorPermissionItemViewModel>();
    public bool IsAdmin { get; set; }
    public int? TargetUserId { get; set; }
    public List<UserOptionViewModel> AllUsers { get; set; } = new List<UserOptionViewModel>();
    public string SelectedUserName { get; set; } = string.Empty;
}

public class UserOptionViewModel
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
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

