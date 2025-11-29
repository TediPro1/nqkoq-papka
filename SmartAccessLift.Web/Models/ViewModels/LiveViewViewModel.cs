namespace SmartAccessLift.Web.Models.ViewModels;

public class LiveViewViewModel
{
    public string CameraFeedUrl { get; set; } = string.Empty;
    public List<OccupantViewModel> CurrentOccupants { get; set; } = new List<OccupantViewModel>();
}

public class OccupantViewModel
{
    public int? UserId { get; set; }
    public string? UserName { get; set; }
    public string? VisitorName { get; set; }
    public DateTime EntryTime { get; set; }
    public int? CurrentFloor { get; set; }
    public string AccessMethod { get; set; } = string.Empty;
}

