// Visitor access form handling

document.addEventListener('DOMContentLoaded', function() {
    const form = document.getElementById('visitorAccessForm');
    if (form) {
        form.addEventListener('submit', handleFormSubmit);
    }
});

async function handleFormSubmit(e) {
    e.preventDefault();

    const formData = {
        visitorName: document.getElementById('VisitorName').value,
        startTime: new Date(document.getElementById('StartTime').value).toISOString(),
        endTime: new Date(document.getElementById('EndTime').value).toISOString(),
        floorIds: Array.from(document.querySelectorAll('input[name="FloorIds"]:checked'))
            .map(cb => parseInt(cb.value))
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

        const result = await response.json();

        if (result.success) {
            // Load QR code modal
            const qrCodeContent = document.getElementById('qrCodeContent');
            const qrResponse = await fetch(`/VisitorAccess/QRCode/${result.visitorAccessId}`);
            qrCodeContent.innerHTML = await qrResponse.text();
            openModal('qrCodeModal');
        } else {
            alert(result.error || 'Failed to create visitor access');
        }
    } catch (error) {
        console.error('Error:', error);
        alert('An error occurred. Please try again.');
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

