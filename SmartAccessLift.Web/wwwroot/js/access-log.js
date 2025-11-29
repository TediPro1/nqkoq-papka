// Access log filtering and pagination

let currentPage = 1;
const pageSize = 50;

document.addEventListener('DOMContentLoaded', function() {
    loadAccessLogs();
});

function applyFilters() {
    currentPage = 1;
    loadAccessLogs();
}

async function loadAccessLogs() {
    const floorId = document.getElementById('floorFilter')?.value || '';
    const accessMethod = document.getElementById('methodFilter')?.value || '';
    const outcome = document.getElementById('outcomeFilter')?.value || '';
    const startDate = document.getElementById('startDateFilter')?.value || '';
    const endDate = document.getElementById('endDateFilter')?.value || '';

    const params = new URLSearchParams({
        page: currentPage,
        pageSize: pageSize
    });

    if (floorId) params.append('floorId', floorId);
    if (accessMethod) params.append('accessMethod', accessMethod);
    if (outcome) params.append('outcome', outcome);
    if (startDate) params.append('startDate', startDate);
    if (endDate) params.append('endDate', endDate);

    try {
        const response = await fetch(`/AccessLog/Data?${params}`);
        const data = await response.json();

        renderLogs(data.logs);
        renderPagination(data);
    } catch (error) {
        console.error('Error loading access logs:', error);
    }
}

function renderLogs(logs) {
    const tbody = document.getElementById('logsTableBody');
    if (!tbody) return;

    if (logs.length === 0) {
        tbody.innerHTML = '<tr><td colspan="5" style="text-align: center; padding: 2rem; color: var(--text-light);">No access logs found</td></tr>';
        return;
    }

    tbody.innerHTML = logs.map(log => `
        <tr>
            <td>${log.userName || log.visitorName || 'Unknown'}</td>
            <td>Floor ${log.floorNumber}</td>
            <td>${log.accessMethod}</td>
            <td>${new Date(log.timestamp).toLocaleString()}</td>
            <td>
                <span class="status-icon ${log.outcome.toLowerCase()}"></span>
                ${log.outcome}
                ${log.reason ? `<br><small style="color: var(--text-light);">${log.reason}</small>` : ''}
            </td>
        </tr>
    `).join('');
}

function renderPagination(data) {
    const pagination = document.getElementById('pagination');
    if (!pagination) return;

    if (data.totalPages <= 1) {
        pagination.innerHTML = '';
        return;
    }

    let html = '<div style="display: flex; justify-content: center; gap: 0.5rem; align-items: center;">';
    
    if (data.page > 1) {
        html += `<button onclick="goToPage(${data.page - 1})" class="btn">Previous</button>`;
    }

    html += `<span>Page ${data.page} of ${data.totalPages}</span>`;

    if (data.page < data.totalPages) {
        html += `<button onclick="goToPage(${data.page + 1})" class="btn">Next</button>`;
    }

    html += '</div>';
    pagination.innerHTML = html;
}

function goToPage(page) {
    currentPage = page;
    loadAccessLogs();
    window.scrollTo({ top: 0, behavior: 'smooth' });
}

