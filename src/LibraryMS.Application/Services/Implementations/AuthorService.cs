using LibraryMS.Application.Common;
using LibraryMS.Application.DTOs.Authors;
using LibraryMS.Application.Services.Interfaces;
using LibraryMS.Application.ViewModels;
using LibraryMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryMS.Application.Services.Implementations;

public class AuthorService : IAuthorService
{
    private readonly ILibraryDbContext _context;
    private readonly IActivityLogService _log;

    public AuthorService(ILibraryDbContext context, IActivityLogService log)
    {
        _context = context;
        _log = log;
    }

    public async Task<ServiceResult<List<AuthorViewModel>>> GetAllAsync()
    {
        var authors = await _context.Authors
            .Select(a => MapToViewModel(a))
            .ToListAsync();
        return ServiceResult<List<AuthorViewModel>>.Ok(authors);
    }

    public async Task<ServiceResult<AuthorViewModel>> GetByIdAsync(int id)
    {
        var author = await _context.Authors.FindAsync(id);
        if (author is null)
            return ServiceResult<AuthorViewModel>.NotFound(
                $"No author found with ID {id}. It may have been deleted or never existed.");
        return ServiceResult<AuthorViewModel>.Ok(MapToViewModel(author));
    }

    public async Task<ServiceResult<AuthorViewModel>> CreateAsync(CreateAuthorDto dto, int currentUserId)
    {
        var author = new Author
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Bio = dto.Bio,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = currentUserId.ToString()
        };
        _context.Authors.Add(author);
        await _context.SaveChangesAsync();
        await _log.LogAsync(currentUserId, "CreateAuthor", "Author", author.Id.ToString(),
            new { author.FirstName, author.LastName });
        return ServiceResult<AuthorViewModel>.Created(MapToViewModel(author), "Author created successfully.");
    }

    public async Task<ServiceResult<AuthorViewModel>> UpdateAsync(int id, UpdateAuthorDto dto, int currentUserId)
    {
        var author = await _context.Authors.FindAsync(id);
        if (author is null)
            return ServiceResult<AuthorViewModel>.NotFound(
                $"No author found with ID {id}. It may have been deleted or never existed.");

        author.FirstName = dto.FirstName;
        author.LastName = dto.LastName;
        author.Bio = dto.Bio;
        author.UpdatedAt = DateTime.UtcNow;
        author.UpdatedBy = currentUserId.ToString();
        await _context.SaveChangesAsync();
        await _log.LogAsync(currentUserId, "UpdateAuthor", "Author", id.ToString(),
            new { dto.FirstName, dto.LastName });
        return ServiceResult<AuthorViewModel>.Ok(MapToViewModel(author), "Author updated successfully.");
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int id, int currentUserId)
    {
        var author = await _context.Authors.FindAsync(id);
        if (author is null)
            return ServiceResult<bool>.NotFound(
                $"No author found with ID {id}. It may have been deleted or never existed.");

        author.IsDeleted = true;
        author.UpdatedAt = DateTime.UtcNow;
        author.UpdatedBy = currentUserId.ToString();
        await _context.SaveChangesAsync();
        await _log.LogAsync(currentUserId, "DeleteAuthor", "Author", id.ToString(), null);
        return ServiceResult<bool>.Ok(true, "Author deleted successfully.");
    }

    private static AuthorViewModel MapToViewModel(Author a) => new()
    {
        Id = a.Id,
        FirstName = a.FirstName,
        LastName = a.LastName,
        Bio = a.Bio,
        CreatedAt = a.CreatedAt
    };
}
