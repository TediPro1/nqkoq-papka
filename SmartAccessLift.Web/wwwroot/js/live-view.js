// Live view video handling and SignalR connection

let connection = null;

document.addEventListener('DOMContentLoaded', function() {
    initializeVideoFeed();
    loadOccupants();
    initializeSignalRConnection();
});

function initializeVideoFeed() {
    const cameraFeed = document.getElementById('cameraFeed');
    if (cameraFeed) {
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

function updateOccupantsList(occupants) {
    const container = document.getElementById('occupantsList');
    if (!container) return;

    if (occupants.length === 0) {
        container.innerHTML = '<p style="color: var(--text-light); text-align: center; padding: 2rem;">No one in elevator</p>';
        return;
    }

    container.innerHTML = occupants.map(occupant => `
        <div class="occupant-item" style="display: flex; align-items: center; gap: 1rem; padding: 1rem; border-bottom: 1px solid #E5E7EB;">
            <div style="width: 40px; height: 40px; border-radius: 50%; background: var(--primary); display: flex; align-items: center; justify-content: center; color: white; font-weight: 600;">
                ${(occupant.userName || occupant.visitorName || '?').charAt(0).toUpperCase()}
            </div>
            <div style="flex: 1;">
                <div style="font-weight: 600;">${occupant.userName || occupant.visitorName || 'Unknown'}</div>
                <div style="font-size: 0.875rem; color: var(--text-light);">
                    ${new Date(occupant.entryTime).toLocaleTimeString()} • ${occupant.accessMethod}
                </div>
            </div>
        </div>
    `).join('');
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

