namespace SmartAccessLift.Web.Services;

public class CameraService : ICameraService
{
    private readonly IConfiguration _configuration;

    public CameraService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GetCameraFeedUrl()
    {
        // TODO: Get actual camera feed URL from configuration or hardware integration
        return _configuration["Camera:FeedUrl"] ?? "/api/camera/feed";
    }

    public Task<bool> IsCameraConnectedAsync()
    {
        // TODO: Implement actual camera connection check
        return Task.FromResult(true);
    }
}

