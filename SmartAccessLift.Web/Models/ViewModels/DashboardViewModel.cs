using SmartAccessLift.Web.Models.ViewModels;

namespace SmartAccessLift.Web.Models.ViewModels;

public class DashboardViewModel
{
    public string UserName { get; set; } = string.Empty;
    public string CameraFeedUrl { get; set; } = string.Empty;
    public List<UpcomingVisitorViewModel> UpcomingVisitors { get; set; } = new List<UpcomingVisitorViewModel>();
    public int ActiveVisitorCount { get; set; }
    public int PendingVisitorCount { get; set; }
}

public class UpcomingVisitorViewModel
{
    public int Id { get; set; }
    public string VisitorName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Status { get; set; } = "Pending"; // "Pending", "Active", "Expired"
    public List<int> Floors { get; set; } = new List<int>();
}

