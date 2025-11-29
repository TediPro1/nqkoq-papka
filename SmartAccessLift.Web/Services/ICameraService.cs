namespace SmartAccessLift.Web.Services;

public interface ICameraService
{
    string GetCameraFeedUrl();
    Task<bool> IsCameraConnectedAsync();
}

