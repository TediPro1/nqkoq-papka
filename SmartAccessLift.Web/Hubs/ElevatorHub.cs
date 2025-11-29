using Microsoft.AspNetCore.SignalR;

namespace SmartAccessLift.Web.Hubs;

public class ElevatorHub : Hub
{
    public async Task JoinElevatorGroup()
    {
        // Add user to elevator monitoring group based on permissions
        await Groups.AddToGroupAsync(Context.ConnectionId, "elevator-monitors");
    }

    // Server methods to call from services
    public async Task NotifyOccupantEntered(object occupantData)
    {
        await Clients.Group("elevator-monitors").SendAsync("OccupantEntered", occupantData);
    }

    public async Task NotifyOccupantExited(object occupantData)
    {
        await Clients.Group("elevator-monitors").SendAsync("OccupantExited", occupantData);
    }

    public async Task NotifyVisitorStatusChanged(object statusData)
    {
        await Clients.Group("elevator-monitors").SendAsync("VisitorStatusChanged", statusData);
    }

    public async Task NotifyCameraFeedUpdated(object feedData)
    {
        await Clients.Group("elevator-monitors").SendAsync("CameraFeedUpdated", feedData);
    }
}

