namespace SmartAccessLift.Web.Models.ViewModels;

public class AccessLogViewModel
{
    public List<AccessLogEntryViewModel> Logs { get; set; } = new List<AccessLogEntryViewModel>();
    public int TotalCount { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public int TotalPages { get; set; }
    public AccessLogFilterViewModel Filter { get; set; } = new AccessLogFilterViewModel();
}

public class AccessLogEntryViewModel
{
    public long Id { get; set; }
    public string? UserName { get; set; }
    public string? VisitorName { get; set; }
    public int FloorNumber { get; set; }
    public string AccessMethod { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Outcome { get; set; } = string.Empty;
    public string? Reason { get; set; }
}

public class AccessLogFilterViewModel
{
    public int? FloorId { get; set; }
    public string? AccessMethod { get; set; }
    public string? Outcome { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? UserId { get; set; }
}

