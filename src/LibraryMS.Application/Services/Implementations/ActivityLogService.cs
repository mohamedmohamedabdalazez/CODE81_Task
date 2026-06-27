using System.Text.Json;
using LibraryMS.Application.Common;
using LibraryMS.Application.Services.Interfaces;
using LibraryMS.Application.ViewModels;
using LibraryMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryMS.Application.Services.Implementations;

public class ActivityLogService : IActivityLogService
{
    private readonly ILibraryDbContext _context;

    public ActivityLogService(ILibraryDbContext context)
    {
        _context = context;
    }

    public async Task LogAsync(int userId, string action, string entityType, string? entityId, object? details = null)
    {
        var log = new ActivityLog
        {
            UserId = userId,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            Timestamp = DateTime.UtcNow,
            Details = details is null ? null : JsonSerializer.Serialize(details)
        };
        _context.ActivityLogs.Add(log);
        await _context.SaveChangesAsync();
    }

    public async Task<ServiceResult<List<ActivityLogViewModel>>> GetAllAsync()
    {
        var logs = await _context.ActivityLogs
            .OrderByDescending(l => l.Timestamp)
            .Select(l => new ActivityLogViewModel
            {
                Id = l.Id,
                UserId = l.UserId,
                Action = l.Action,
                EntityType = l.EntityType,
                EntityId = l.EntityId,
                Timestamp = l.Timestamp,
                Details = l.Details
            })
            .ToListAsync();
        return ServiceResult<List<ActivityLogViewModel>>.Ok(logs);
    }

    public async Task<ServiceResult<List<ActivityLogViewModel>>> GetByUserAsync(int userId)
    {
        var logs = await _context.ActivityLogs
            .Where(l => l.UserId == userId)
            .OrderByDescending(l => l.Timestamp)
            .Select(l => new ActivityLogViewModel
            {
                Id = l.Id,
                UserId = l.UserId,
                Action = l.Action,
                EntityType = l.EntityType,
                EntityId = l.EntityId,
                Timestamp = l.Timestamp,
                Details = l.Details
            })
            .ToListAsync();
        return ServiceResult<List<ActivityLogViewModel>>.Ok(logs);
    }
}
