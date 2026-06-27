using LibraryMS.Application.Common;
using LibraryMS.Application.ViewModels;

namespace LibraryMS.Application.Services.Interfaces;

public interface IActivityLogService
{
    Task LogAsync(int userId, string action, string entityType, string? entityId, object? details = null);
    Task<ServiceResult<List<ActivityLogViewModel>>> GetAllAsync();
    Task<ServiceResult<List<ActivityLogViewModel>>> GetByUserAsync(int userId);
}
