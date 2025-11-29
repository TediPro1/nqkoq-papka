// Floor permission toggles

document.addEventListener('DOMContentLoaded', function() {
    initializeToggles();
    setupSaveButton();
});

function initializeToggles() {
    const toggles = document.querySelectorAll('.toggle-switch input[type="checkbox"]');
    toggles.forEach(toggle => {
        toggle.addEventListener('change', function() {
            updateVisualIndicator(this);
        });
    });
}

function updateVisualIndicator(toggle) {
    const floorItem = toggle.closest('.floor-item');
    const indicator = floorItem.querySelector('.floor-indicator');
    
    if (toggle.checked) {
        indicator.classList.remove('restricted');
        indicator.classList.add('allowed');
    } else {
        indicator.classList.remove('allowed');
        indicator.classList.add('restricted');
    }
}

function setupSaveButton() {
    const saveBtn = document.getElementById('savePermissionsBtn');
    if (saveBtn) {
        saveBtn.addEventListener('click', savePermissions);
    }
}

async function savePermissions() {
    const toggles = document.querySelectorAll('.toggle-switch input[type="checkbox"]');
    const permissions = Array.from(toggles).map(toggle => ({
        floorId: parseInt(toggle.dataset.floorId),
        isAllowed: toggle.checked
    }));

    try {
        const response = await fetch('/FloorPermission/Update', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
            },
            body: JSON.stringify({ permissions: permissions })
        });

        const result = await response.json();

        if (result.success) {
            showToast('Permissions saved successfully!', 'success');
        } else {
            showToast(result.errors?.[0] || 'Failed to save permissions', 'error');
        }
    } catch (error) {
        console.error('Error:', error);
        showToast('An error occurred. Please try again.', 'error');
    }
}

