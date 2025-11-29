// Visitor access form handling

document.addEventListener('DOMContentLoaded', function() {
    const form = document.getElementById('visitorAccessForm');
    if (form) {
        form.addEventListener('submit', handleFormSubmit);
    }
});

async function handleFormSubmit(e) {
    e.preventDefault();

    // Get all checked floor checkboxes
    const floorCheckboxes = document.querySelectorAll('input[name="FloorIds"]:checked');
    const floorIds = Array.from(floorCheckboxes).map(checkbox => parseInt(checkbox.value));

    const formData = {
        visitorName: document.getElementById('VisitorName').value,
        startTime: document.getElementById('StartTime').value, // Keep as string, let server parse
        endTime: document.getElementById('EndTime').value,     // Keep as string, let server parse
        floorIds: floorIds
    };

    if (formData.floorIds.length === 0) {
        alert('Please select at least one floor');
        return;
    }

    try {
        const response = await fetch('/VisitorAccess/Create', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
            },
            body: JSON.stringify(formData)
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const result = await response.json();

        if (result.success) {
            // Display the custom QR code image
            const qrCodeContent = document.getElementById('qrCodeContent');
            qrCodeContent.innerHTML = `
                <div style="text-align: center; padding: 1rem;">
                    <img src="/images/qr-code.png" alt="QR Code" style="max-width: 300px; height: auto; margin: 0 auto 1rem; display: block;">
                    <div class="qr-actions" style="display: flex; gap: 1rem; justify-content: center; margin-top: 1rem;">
                        <button onclick="copyLink('${result.shareableLink}')" class="btn btn-secondary" style="display: flex; align-items: center; gap: 0.5rem;">
                            <i data-lucide="copy" style="width: 16px; height: 16px;"></i>
                            Copy Link
                        </button>
                        <button onclick="shareLink('${result.shareableLink}')" class="btn btn-primary" style="display: flex; align-items: center; gap: 0.5rem;">
                            <i data-lucide="share-2" style="width: 16px; height: 16px;"></i>
                            Share
                        </button>
                    </div>
                </div>
            `;
            lucide.createIcons();
            openModal('qrCodeModal');
        } else {
            throw new Error(result.error || 'Failed to create visitor access');
        }
    } catch (error) {
        console.error('Error:', error);
        alert(error.message || 'An error occurred. Please try again.');
    }
}

function copyLink(link) {
    navigator.clipboard.writeText(link).then(() => {
        showToast('Link copied to clipboard!', 'success');
    }).catch(err => {
        console.error('Failed to copy:', err);
    });
}

function shareLink(link) {
    if (navigator.share) {
        navigator.share({
            title: 'SmartAccess Lift Visitor Access',
            text: 'Use this link to access the elevator',
            url: link
        }).catch(err => console.error('Error sharing:', err));
    } else {
        copyLink(link);
    }
}

