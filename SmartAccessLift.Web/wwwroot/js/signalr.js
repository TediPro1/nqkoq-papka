// SignalR client connection
let connection = null;

function initializeSignalR() {
    if (typeof signalR !== 'undefined') {
        connection = new signalR.HubConnectionBuilder()
            .withUrl("/hubs/elevator")
            .withAutomaticReconnect()
            .build();

        connection.start()
            .then(() => {
                console.log("SignalR Connected");
                connection.invoke("JoinElevatorGroup");
            })
            .catch(err => console.error("SignalR Connection Error: ", err));

        // Handle reconnection
        connection.onreconnecting(() => {
            console.log("SignalR Reconnecting...");
        });

        connection.onreconnected(() => {
            console.log("SignalR Reconnected");
            connection.invoke("JoinElevatorGroup");
        });

        // Listen for events
        connection.on("OccupantEntered", (data) => {
            if (typeof updateOccupantsList === 'function') {
                updateOccupantsList(data);
            }
        });

        connection.on("OccupantExited", (data) => {
            if (typeof updateOccupantsList === 'function') {
                updateOccupantsList(data);
            }
        });

        connection.on("VisitorStatusChanged", (data) => {
            if (typeof updateVisitorStatus === 'function') {
                updateVisitorStatus(data);
            }
        });

        connection.on("CameraFeedUpdated", (data) => {
            if (typeof updateCameraFeed === 'function') {
                updateCameraFeed(data);
            }
        });
    }
}

// Initialize on page load
document.addEventListener('DOMContentLoaded', initializeSignalR);

