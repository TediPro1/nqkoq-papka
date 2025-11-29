// Floor permission toggles

document.addEventListener('DOMContentLoaded', function() {
    initializeToggles();
    setupSaveButton();
    setupUserSelector();
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

function setupUserSelector() {
    const userSelector = document.getElementById('userSelector');
    if (userSelector) {
        userSelector.addEventListener('change', function() {
            const selectedUserId = this.value;
            // Reload the page with the selected user ID
            window.location.href = `/FloorPermission/Index?userId=${selectedUserId}`;
        });
    }
}

async function savePermissions() {
    const toggles = document.querySelectorAll('.toggle-switch input[type="checkbox"]');
    const permissions = Array.from(toggles).map(toggle => ({
        floorId: parseInt(toggle.dataset.floorId),
        isAllowed: toggle.checked
    }));

    const targetUserId = document.getElementById('targetUserId')?.value || 
                         document.querySelector('input[name="TargetUserId"]')?.value || null;

    try {
        const response = await fetch('/FloorPermission/Update', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
            },
            body: JSON.stringify({
                userId: targetUserId ? parseInt(targetUserId) : null,
                permissions: permissions
            })
        });

        const result = await response.json();
        
        if (result.success) {
            showToast('Permissions updated successfully!', 'success');
        } else {
            showToast(result.errors ? result.errors.join('\n') : 'Failed to update permissions', 'error');
        }
    } catch (error) {
        console.error('Error:', error);
        showToast('An error occurred while saving permissions', 'error');
    }
}
