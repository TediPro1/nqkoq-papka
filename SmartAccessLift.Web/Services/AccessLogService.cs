using Microsoft.EntityFrameworkCore;
using SmartAccessLift.Web.Data;
using SmartAccessLift.Web.Models.Entities;

namespace SmartAccessLift.Web.Services;

public class AccessLogService : IAccessLogService
{
    private readonly ApplicationDbContext _context;

    public AccessLogService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<AccessLog>> GetAccessLogsAsync(int? userId, int? floorId, string? accessMethod, string? outcome, DateTime? startDate, DateTime? endDate, int page = 1, int pageSize = 50)
    {
        var query = _context.AccessLogs.AsQueryable();

        if (userId.HasValue)
        {
            query = query.Where(log => log.UserId == userId);
        }

        if (floorId.HasValue)
        {
            query = query.Where(log => log.FloorId == floorId);
        }

        if (!string.IsNullOrEmpty(accessMethod))
        {
            query = query.Where(log => log.AccessMethod == accessMethod);
        }

        if (!string.IsNullOrEmpty(outcome))
        {
            query = query.Where(log => log.Outcome == outcome);
        }

        if (startDate.HasValue)
        {
            query = query.Where(log => log.Timestamp >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(log => log.Timestamp <= endDate.Value);
        }

        return await query
            .Include(log => log.User)
            .Include(log => log.VisitorAccess)
            .Include(log => log.Floor)
            .OrderByDescending(log => log.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<long> GetAccessLogCountAsync(int? userId, int? floorId, string? accessMethod, string? outcome, DateTime? startDate, DateTime? endDate)
    {
        var query = _context.AccessLogs.AsQueryable();

        if (userId.HasValue)
        {
            query = query.Where(log => log.UserId == userId);
        }

        if (floorId.HasValue)
        {
            query = query.Where(log => log.FloorId == floorId);
        }

        if (!string.IsNullOrEmpty(accessMethod))
        {
            query = query.Where(log => log.AccessMethod == accessMethod);
        }

        if (!string.IsNullOrEmpty(outcome))
        {
            query = query.Where(log => log.Outcome == outcome);
        }

        if (startDate.HasValue)
        {
            query = query.Where(log => log.Timestamp >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(log => log.Timestamp <= endDate.Value);
        }

        return await query.LongCountAsync();
    }
}

