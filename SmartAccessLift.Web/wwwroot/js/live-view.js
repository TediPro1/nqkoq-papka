// Live view video handling and SignalR connection

let connection = null;

document.addEventListener('DOMContentLoaded', function() {
    initializeVideoFeed();
    loadOccupants();
    initializeSignalRConnection();
    
    // Initialize floor and door status
    // Doors start as Open, will be updated by video time or SignalR
    updateFloorNumber(1);
    updateDoorStatus('Open');
});

function initializeVideoFeed() {
    const cameraFeed = document.getElementById('cameraFeed');
    if (cameraFeed) {
        // Handle video errors
        cameraFeed.addEventListener('error', function() {
            const container = cameraFeed.parentElement;
            container.innerHTML = `
                <div style="position: absolute; top: 50%; left: 50%; transform: translate(-50%, -50%); text-align: center; color: white;">
                    <i data-lucide="wifi-off" style="width: 48px; height: 48px; margin-bottom: 1rem;"></i>
                    <p style="margin-bottom: 1rem;">Camera feed connection lost</p>
                    <button onclick="location.reload()" class="btn btn-primary">Retry</button>
                </div>
            `;
            lucide.createIcons();
        });

        // Track if doors have been closed (to prevent multiple updates)
        let doorsClosed = false;

        // Listen for time updates to change door status at 7.5 seconds
        cameraFeed.addEventListener('timeupdate', function() {
            const currentTime = cameraFeed.currentTime;
            
            // When video reaches 7.5 seconds, close the doors
            if (currentTime >= 7.5 && !doorsClosed) {
                updateDoorStatus('Closed');
                doorsClosed = true;
            }
            
            // Reset when video loops back (if it loops before 7.5 seconds)
            if (currentTime < 7.5 && doorsClosed) {
                updateDoorStatus('Open');
                doorsClosed = false;
            }
        });

        // Reset door status when video loops
        cameraFeed.addEventListener('ended', function() {
            updateDoorStatus('Open');
            doorsClosed = false;
        });
    }
}

async function loadOccupants() {
    try {
        const response = await fetch('/LiveView/Occupants');
        const data = await response.json();
        updateOccupantsList(data.occupants);
    } catch (error) {
        console.error('Error loading occupants:', error);
    }
}

function formatEntryTime(dateTimeString) {
    const date = new Date(dateTimeString);
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    const hours = String(date.getHours()).padStart(2, '0');
    const minutes = String(date.getMinutes()).padStart(2, '0');
    return `${year}/${month}/${day}:${hours}:${minutes}`;
}

function updateOccupantsList(occupants) {
    const container = document.getElementById('occupantsList');
    if (!container) return;

    if (occupants.length === 0) {
        container.innerHTML = '<p style="color: #9CA3AF; text-align: center; padding: 2rem; grid-column: 1 / -1;">No one in elevator</p>';
        return;
    }

    container.innerHTML = occupants.map(occupant => `
        <div class="occupant-item" style="background: #1F2937; border-radius: 12px; padding: 1.5rem; display: flex; align-items: center; gap: 1rem;">
            <div style="width: 48px; height: 48px; border-radius: 50%; background: #4B5563; display: flex; align-items: center; justify-content: center; color: white; font-weight: 600; font-size: 1.25rem;">
                ${(occupant.userName || occupant.visitorName || '?').charAt(0).toUpperCase()}
            </div>
            <div style="flex: 1;">
                <div style="font-weight: 600; color: white; margin-bottom: 0.25rem;">${occupant.userName || occupant.visitorName || 'Unknown'}</div>
                <div style="font-size: 0.875rem; color: #9CA3AF;">
                    ${formatEntryTime(occupant.entryTime)} • ${occupant.accessMethod}
                </div>
            </div>
        </div>
    `).join('');
}

function updateFloorNumber(floor) {
    const floorElement = document.getElementById('currentFloor');
    if (floorElement) {
        floorElement.textContent = floor !== null && floor !== undefined ? floor : '-';
    }
}

function updateDoorStatus(status) {
    const doorStateElement = document.getElementById('doorState');
    const doorImageElement = document.getElementById('doorImage');
    
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

function initializeSignalRConnection() {
    if (typeof signalR !== 'undefined') {
        connection = new signalR.HubConnectionBuilder()
            .withUrl("/hubs/elevator")
            .withAutomaticReconnect()
            .build();

        connection.start()
            .then(() => {
                console.log("SignalR Connected for Live View");
                connection.invoke("JoinElevatorGroup");
            })
            .catch(err => console.error("SignalR Connection Error: ", err));

        connection.on("OccupantEntered", (data) => {
            loadOccupants(); // Reload to get updated list
        });

        connection.on("OccupantExited", (data) => {
            loadOccupants(); // Reload to get updated list
        });

        connection.on("FloorChanged", (data) => {
            if (data.floor !== undefined) {
                updateFloorNumber(data.floor);
            }
        });

        connection.on("DoorStatusChanged", (data) => {
            if (data.status) {
                updateDoorStatus(data.status);
            }
        });
    }
}

// Function to be called by SignalR
function updateCameraFeed(data) {
    if (data.status === 'error') {
        const cameraFeed = document.getElementById('cameraFeed');
        if (cameraFeed) {
            cameraFeed.dispatchEvent(new Event('error'));
        }
    }
}

