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

    // Convert datetime-local to ISO 8601 format for proper parsing
    const startTimeValue = document.getElementById('StartTime').value;
    const endTimeValue = document.getElementById('EndTime').value;
    
    // datetime-local format is "YYYY-MM-DDTHH:mm", convert to ISO 8601
    const startTimeISO = startTimeValue ? new Date(startTimeValue).toISOString() : null;
    const endTimeISO = endTimeValue ? new Date(endTimeValue).toISOString() : null;

    const formData = {
        VisitorName: document.getElementById('VisitorName').value,
        StartTime: startTimeISO,
        EndTime: endTimeISO,
        FloorIds: floorIds
    };

    // Validate required fields
    if (!formData.VisitorName || formData.VisitorName.trim() === '') {
        alert('Please enter a visitor name');
        return;
    }

    if (!formData.StartTime || !formData.EndTime) {
        alert('Please select both start and end times');
        return;
    }

    if (formData.FloorIds.length === 0) {
        alert('Please select at least one floor');
        return;
    }

    try {
        console.log('Submitting form data:', formData);
        const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';
        const response = await fetch('/VisitorAccess/Create', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': token
            },
            body: JSON.stringify(formData)
        });

        console.log('Response status:', response.status);

        const result = await response.json();

        if (!response.ok) {
            // Try to get error details from the response
            let errorMessage = `HTTP error! status: ${response.status}`;
            if (result.error) {
                errorMessage = result.error;
            } else if (result.errors) {
                errorMessage = Array.isArray(result.errors) ? result.errors.join(', ') : result.errors;
            } else if (result.message) {
                errorMessage = result.message;
            }
            throw new Error(errorMessage);
        }

        if (result.success) {
            // Display the QR code image from the server response
            const qrCodeContent = document.getElementById('qrCodeContent');
            const qrCodeImageUrl = result.qrCodeImage || '/images/download.png';
            const visitorName = formData.VisitorName || 'visitor';
            
            console.log('QR Code Image URL length:', qrCodeImageUrl ? qrCodeImageUrl.length : 0);
            console.log('QR Code Image URL preview:', qrCodeImageUrl ? qrCodeImageUrl.substring(0, 100) + '...' : 'null');
            
            // Store visitor name for download function
            window.currentVisitorName = visitorName;
            
            // Escape the image URL for use in HTML
            const escapedImageUrl = qrCodeImageUrl.replace(/'/g, "\\'").replace(/"/g, '&quot;');
            
            // Generate random access code if not provided
            const accessCode = result.accessCode || generateRandomCode();
            
            qrCodeContent.innerHTML = `
                <div style="text-align: center; padding: 1rem;">
                    <img id="qrCodeImage" src="${escapedImageUrl}" alt="QR Code" 
                         onerror="console.error('Failed to load QR code image'); this.src='/images/download.png';" 
                         style="max-width: 300px; height: auto; margin: 0 auto 1rem; display: block; border: 1px solid var(--border-color); border-radius: 8px; padding: 1rem; background: white;">
                    <div style="margin: 1.5rem 0; padding: 1rem; background: var(--bg-light); border-radius: 8px;">
                        <div style="font-size: 0.875rem; color: var(--text-secondary); margin-bottom: 0.5rem;">Access Code</div>
                        <div id="accessCode" style="font-size: 2rem; font-weight: 700; color: var(--text-dark); letter-spacing: 0.2em; font-family: monospace;">${accessCode}</div>
                        <button onclick="copyAccessCode('${accessCode}')" class="btn btn-secondary" style="margin-top: 0.5rem; padding: 0.5rem 1rem; font-size: 0.875rem;">
                            <i data-lucide="copy" style="width: 14px; height: 14px; margin-right: 0.25rem;"></i>
                            Copy Code
                        </button>
                    </div>
                    <div class="qr-actions" style="display: flex; gap: 1rem; justify-content: center; margin-top: 1rem; flex-wrap: wrap;">
                        <button onclick="downloadQRCode('${escapedImageUrl}')" class="btn btn-primary" style="display: flex; align-items: center; gap: 0.5rem;">
                            <i data-lucide="download" style="width: 16px; height: 16px;"></i>
                            Download QR Code
                        </button>
                        <button onclick="copyLink('${result.shareableLink}')" class="btn btn-secondary" style="display: flex; align-items: center; gap: 0.5rem;">
                            <i data-lucide="copy" style="width: 16px; height: 16px;"></i>
                            Copy Link
                        </button>
                        <button onclick="shareLink('${result.shareableLink}')" class="btn btn-secondary" style="display: flex; align-items: center; gap: 0.5rem;">
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

function generateRandomCode() {
    // Generate a 6-digit random code
    return Math.floor(100000 + Math.random() * 900000).toString();
}

function copyAccessCode(code) {
    navigator.clipboard.writeText(code).then(() => {
        showToast('Access code copied to clipboard!', 'success');
    }).catch(err => {
        console.error('Failed to copy:', err);
    });
}

async function downloadQRCode(imageUrl) {
    try {
        const visitorName = window.currentVisitorName || 'visitor';
        const sanitizedName = visitorName.replace(/[^a-zA-Z0-9_-]/g, '_');
        
        // Handle base64 data URLs
        if (imageUrl.startsWith('data:')) {
            // Convert base64 data URL to blob
            const base64Data = imageUrl.split(',')[1];
            const byteCharacters = atob(base64Data);
            const byteNumbers = new Array(byteCharacters.length);
            for (let i = 0; i < byteCharacters.length; i++) {
                byteNumbers[i] = byteCharacters.charCodeAt(i);
            }
            const byteArray = new Uint8Array(byteNumbers);
            const blob = new Blob([byteArray], { type: 'image/png' });
            
            const url = window.URL.createObjectURL(blob);
            const link = document.createElement('a');
            link.href = url;
            link.download = `QRCode_${sanitizedName}_${Date.now()}.png`;
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
            
            // Clean up the object URL
            setTimeout(() => window.URL.revokeObjectURL(url), 100);
        } else {
            // Regular URL - fetch and download
            const response = await fetch(imageUrl);
            const blob = await response.blob();
            const url = window.URL.createObjectURL(blob);
            
            const link = document.createElement('a');
            link.href = url;
            link.download = `QRCode_${sanitizedName}_${Date.now()}.png`;
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
            
            // Clean up the object URL
            setTimeout(() => window.URL.revokeObjectURL(url), 100);
        }
        
        showToast('QR Code downloaded successfully!', 'success');
    } catch (error) {
        console.error('Error downloading QR code:', error);
        showToast('Failed to download QR code', 'error');
    }
}

