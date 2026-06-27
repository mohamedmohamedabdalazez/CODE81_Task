using BCrypt.Net;
using LibraryMS.Application.Common;
using LibraryMS.Application.DTOs.Users;
using LibraryMS.Application.Services.Interfaces;
using LibraryMS.Application.ViewModels;
using LibraryMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryMS.Application.Services.Implementations;

public class UserService : IUserService
{
    private readonly ILibraryDbContext _context;
    private readonly IActivityLogService _log;

    public UserService(ILibraryDbContext context, IActivityLogService log)
    {
        _context = context;
        _log = log;
    }

    public async Task<ServiceResult<List<UserViewModel>>> GetAllAsync()
    {
        var users = await _context.SystemUsers
            .Select(u => MapToViewModel(u))
            .ToListAsync();
        return ServiceResult<List<UserViewModel>>.Ok(users);
    }

    public async Task<ServiceResult<UserViewModel>> GetByIdAsync(int id)
    {
        var user = await _context.SystemUsers.FindAsync(id);
        if (user is null)
            return ServiceResult<UserViewModel>.NotFound(
                $"No user found with ID {id}. It may have been deleted or never existed.");
        return ServiceResult<UserViewModel>.Ok(MapToViewModel(user));
    }

    public async Task<ServiceResult<UserViewModel>> CreateAsync(CreateUserDto dto, int currentUserId)
    {
        var usernameExists = await _context.SystemUsers
            .AnyAsync(u => u.Username == dto.Username);
        if (usernameExists)
            return ServiceResult<UserViewModel>.Conflict(
                $"A user with username '{dto.Username}' already exists in the system.");

        var user = new SystemUser
        {
            Username = dto.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = dto.Role,
            FullName = dto.FullName,
            Email = dto.Email,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = currentUserId.ToString()
        };
        _context.SystemUsers.Add(user);
        await _context.SaveChangesAsync();
        await _log.LogAsync(currentUserId, "CreateUser", "SystemUser", user.Id.ToString(),
            new { user.Username, user.Role });
        return ServiceResult<UserViewModel>.Created(MapToViewModel(user), "User created successfully.");
    }

    public async Task<ServiceResult<UserViewModel>> UpdateAsync(int id, UpdateUserDto dto, int currentUserId)
    {
        var user = await _context.SystemUsers.FindAsync(id);
        if (user is null)
            return ServiceResult<UserViewModel>.NotFound(
                $"No user found with ID {id}. It may have been deleted or never existed.");

        user.Role = dto.Role;
        user.FullName = dto.FullName;
        user.Email = dto.Email;
        user.IsActive = dto.IsActive;
        if (!string.IsNullOrWhiteSpace(dto.Password))
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        user.UpdatedAt = DateTime.UtcNow;
        user.UpdatedBy = currentUserId.ToString();
        await _context.SaveChangesAsync();
        await _log.LogAsync(currentUserId, "UpdateUser", "SystemUser", id.ToString(),
            new { user.Username, user.Role });
        return ServiceResult<UserViewModel>.Ok(MapToViewModel(user), "User updated successfully.");
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int id, int currentUserId)
    {
        var user = await _context.SystemUsers.FindAsync(id);
        if (user is null)
            return ServiceResult<bool>.NotFound(
                $"No user found with ID {id}. It may have been deleted or never existed.");

        user.IsDeleted = true;
        user.UpdatedAt = DateTime.UtcNow;
        user.UpdatedBy = currentUserId.ToString();
        await _context.SaveChangesAsync();
        await _log.LogAsync(currentUserId, "DeleteUser", "SystemUser", id.ToString(), null);
        return ServiceResult<bool>.Ok(true, "User deleted successfully.");
    }

    public async Task<SystemUser?> AuthenticateAsync(string username, string password)
    {
        // IgnoreQueryFilters to also check soft-deleted users (to return the deactivated message)
        var user = await _context.SystemUsers
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Username == username);

        if (user is null) return null;
        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)) return null;
        return user;
    }

    private static UserViewModel MapToViewModel(SystemUser u) => new()
    {
        Id = u.Id,
        Username = u.Username,
        Role = u.Role.ToString(),
        FullName = u.FullName,
        Email = u.Email,
        IsActive = u.IsActive,
        CreatedAt = u.CreatedAt
    };
}
