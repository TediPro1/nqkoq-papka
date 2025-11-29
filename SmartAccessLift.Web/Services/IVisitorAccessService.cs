using SmartAccessLift.Web.Models.Entities;
using SmartAccessLift.Web.Models.ViewModels;

namespace SmartAccessLift.Web.Services;

public interface IVisitorAccessService
{
    Task<VisitorAccess> CreateVisitorAccessAsync(CreateVisitorAccessRequest request, int createdByUserId);
    Task<List<VisitorAccess>> GetUserVisitorAccessesAsync(int userId, string? status = null);
    Task<VisitorAccess?> GetVisitorAccessByIdAsync(int id);
    Task UpdateVisitorAccessStatusAsync();
}

