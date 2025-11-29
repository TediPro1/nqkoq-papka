using SmartAccessLift.Web.Models.Entities;

namespace SmartAccessLift.Web.Services;

public interface IAccessLogService
{
    Task<List<AccessLog>> GetAccessLogsAsync(int? userId, int? floorId, string? accessMethod, string? outcome, DateTime? startDate, DateTime? endDate, int page = 1, int pageSize = 50);
    Task<long> GetAccessLogCountAsync(int? userId, int? floorId, string? accessMethod, string? outcome, DateTime? startDate, DateTime? endDate);
}

