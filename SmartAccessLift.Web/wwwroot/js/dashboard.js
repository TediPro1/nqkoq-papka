// Dashboard interactivity

document.addEventListener('DOMContentLoaded', function() {
    initializeCameraFeed();
    updateVisitorStatus();
});

function initializeCameraFeed() {
    const cameraFeed = document.getElementById('cameraFeed');
    if (cameraFeed) {
        cameraFeed.addEventListener('error', function() {
            console.error('Camera feed error');
            // Show error message
            const container = cameraFeed.parentElement;
            container.innerHTML = `
                <div style="position: absolute; top: 50%; left: 50%; transform: translate(-50%, -50%); text-align: center; color: white;">
                    <p>Camera feed unavailable</p>
                    <button onclick="location.reload()" class="btn btn-primary" style="margin-top: 1rem;">Retry</button>
                </div>
            `;
        });
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

