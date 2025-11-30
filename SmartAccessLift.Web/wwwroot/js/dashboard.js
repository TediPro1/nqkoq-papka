// Dashboard interactivity

document.addEventListener('DOMContentLoaded', function() {
    initializeCameraFeed();
    updateVisitorStatus();
    initializeElevatorStatus();
});

function initializeCameraFeed() {
    const cameraFeed = document.getElementById('cameraFeed');
    if (cameraFeed) {
        // Handle video errors
        cameraFeed.addEventListener('error', function() {
            console.error('Camera feed error');
            const container = cameraFeed.parentElement;
            container.innerHTML = `
                <div style="position: absolute; top: 50%; left: 50%; transform: translate(-50%, -50%); text-align: center; color: white;">
                    <p>Camera feed unavailable</p>
                    <button onclick="location.reload()" class="btn btn-primary" style="margin-top: 1rem;">Retry</button>
                </div>
            `;
        });

        // Track if doors have been closed (to prevent multiple updates)
        let doorsClosed = false;

        // Listen for time updates to change door status at 7.5 seconds
        cameraFeed.addEventListener('timeupdate', function() {
            const currentTime = cameraFeed.currentTime;
            
            // When video reaches 7.5 seconds, close the doors
            if (currentTime >= 7.5 && !doorsClosed) {
                updateDashboardDoorStatus('Closed');
                doorsClosed = true;
            }
            
            // Reset when video loops back (if it loops before 7.5 seconds)
            if (currentTime < 7.5 && doorsClosed) {
                updateDashboardDoorStatus('Open');
                doorsClosed = false;
            }
        });

        // Reset door status when video loops
        cameraFeed.addEventListener('ended', function() {
            updateDashboardDoorStatus('Open');
            doorsClosed = false;
        });
    }
}

function initializeElevatorStatus() {
    // Initialize floor and door status
    updateDashboardFloorNumber(1);
    updateDashboardDoorStatus('Open');
}

function updateDashboardFloorNumber(floor) {
    const floorElement = document.getElementById('dashboardCurrentFloor');
    if (floorElement) {
        floorElement.textContent = floor !== null && floor !== undefined ? floor : '-';
    }
}

function updateDashboardDoorStatus(status) {
    const doorStateElement = document.getElementById('dashboardDoorState');
    const doorImageElement = document.getElementById('dashboardDoorImage');
    
    if (doorStateElement) {
        doorStateElement.textContent = status || 'Closed';
    }
    
    if (doorImageElement) {
        // Update the door image based on status
        if (status === 'Open' || status === 'open') {
            doorImageElement.src = '/images/elevator_dors_open.png';
        } else {
            doorImageElement.src = '/images/elevator_dors_closed.png';
        }
    }
}

function updateVisitorStatus() {
    // This will be called via SignalR when visitor status changes
    // For now, just a placeholder
}

// Function to be called by SignalR
function updateVisitorStatusFromSignalR(data) {
    // Update visitor status chips in real-time
    const visitorItems = document.querySelectorAll('.visitor-item');
    visitorItems.forEach(item => {
        const visitorId = item.dataset.visitorId;
        if (visitorId == data.visitorAccessId) {
            const statusChip = item.querySelector('.status-chip');
            if (statusChip) {
                statusChip.className = `status-chip status-${data.status.toLowerCase()}`;
                statusChip.textContent = data.status;
            }
        }
    });
}

