namespace SmartAccessLift.Web.Models.ViewModels;

public class VisitorAccessViewModel
{
    public List<FloorOptionViewModel> AvailableFloors { get; set; } = new List<FloorOptionViewModel>();
}

public class FloorOptionViewModel
{
    public int FloorId { get; set; }
    public int FloorNumber { get; set; }
    public string? FloorName { get; set; }
    public bool IsAllowed { get; set; }
}

public class CreateVisitorAccessRequest
{
    public string VisitorName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public List<int> FloorIds { get; set; } = new List<int>();
}

public class QRCodeViewModel
{
    public int VisitorAccessId { get; set; }
    public string VisitorName { get; set; } = string.Empty;
    public string QRCodeImageUrl { get; set; } = string.Empty;
    public string ShareableLink { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}

