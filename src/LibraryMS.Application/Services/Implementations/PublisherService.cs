using LibraryMS.Application.Common;
using LibraryMS.Application.DTOs.Publishers;
using LibraryMS.Application.Services.Interfaces;
using LibraryMS.Application.ViewModels;
using LibraryMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryMS.Application.Services.Implementations;

public class PublisherService : IPublisherService
{
    private readonly ILibraryDbContext _context;
    private readonly IActivityLogService _log;

    public PublisherService(ILibraryDbContext context, IActivityLogService log)
    {
        _context = context;
        _log = log;
    }

    public async Task<ServiceResult<List<PublisherViewModel>>> GetAllAsync()
    {
        var publishers = await _context.Publishers
            .Select(p => MapToViewModel(p))
            .ToListAsync();
        return ServiceResult<List<PublisherViewModel>>.Ok(publishers);
    }

    public async Task<ServiceResult<PublisherViewModel>> GetByIdAsync(int id)
    {
        var publisher = await _context.Publishers.FindAsync(id);
        if (publisher is null)
            return ServiceResult<PublisherViewModel>.NotFound(
                $"No publisher found with ID {id}. It may have been deleted or never existed.");
        return ServiceResult<PublisherViewModel>.Ok(MapToViewModel(publisher));
    }

    public async Task<ServiceResult<PublisherViewModel>> CreateAsync(CreatePublisherDto dto, int currentUserId)
    {
        var publisher = new Publisher
        {
            Name = dto.Name,
            Address = dto.Address,
            Website = dto.Website,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = currentUserId.ToString()
        };
        _context.Publishers.Add(publisher);
        await _context.SaveChangesAsync();
        await _log.LogAsync(currentUserId, "CreatePublisher", "Publisher", publisher.Id.ToString(),
            new { publisher.Name });
        return ServiceResult<PublisherViewModel>.Created(MapToViewModel(publisher), "Publisher created successfully.");
    }

    public async Task<ServiceResult<PublisherViewModel>> UpdateAsync(int id, UpdatePublisherDto dto, int currentUserId)
    {
        var publisher = await _context.Publishers.FindAsync(id);
        if (publisher is null)
            return ServiceResult<PublisherViewModel>.NotFound(
                $"No publisher found with ID {id}. It may have been deleted or never existed.");

        publisher.Name = dto.Name;
        publisher.Address = dto.Address;
        publisher.Website = dto.Website;
        publisher.UpdatedAt = DateTime.UtcNow;
        publisher.UpdatedBy = currentUserId.ToString();
        await _context.SaveChangesAsync();
        await _log.LogAsync(currentUserId, "UpdatePublisher", "Publisher", id.ToString(), new { dto.Name });
        return ServiceResult<PublisherViewModel>.Ok(MapToViewModel(publisher), "Publisher updated successfully.");
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int id, int currentUserId)
    {
        var publisher = await _context.Publishers.FindAsync(id);
        if (publisher is null)
            return ServiceResult<bool>.NotFound(
                $"No publisher found with ID {id}. It may have been deleted or never existed.");

        publisher.IsDeleted = true;
        publisher.UpdatedAt = DateTime.UtcNow;
        publisher.UpdatedBy = currentUserId.ToString();
        await _context.SaveChangesAsync();
        await _log.LogAsync(currentUserId, "DeletePublisher", "Publisher", id.ToString(), null);
        return ServiceResult<bool>.Ok(true, "Publisher deleted successfully.");
    }

    private static PublisherViewModel MapToViewModel(Publisher p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Address = p.Address,
        Website = p.Website,
        CreatedAt = p.CreatedAt
    };
}
