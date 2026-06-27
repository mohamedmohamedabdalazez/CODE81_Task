using LibraryMS.Application.Common;
using LibraryMS.Application.DTOs.Members;
using LibraryMS.Application.Services.Interfaces;
using LibraryMS.Application.ViewModels;
using LibraryMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryMS.Application.Services.Implementations;

public class MemberService : IMemberService
{
    private readonly ILibraryDbContext _context;
    private readonly IActivityLogService _log;

    public MemberService(ILibraryDbContext context, IActivityLogService log)
    {
        _context = context;
        _log = log;
    }

    public async Task<ServiceResult<List<MemberViewModel>>> GetAllAsync()
    {
        var members = await _context.Members
            .Select(m => MapToViewModel(m))
            .ToListAsync();
        return ServiceResult<List<MemberViewModel>>.Ok(members);
    }

    public async Task<ServiceResult<MemberViewModel>> GetByIdAsync(int id)
    {
        var member = await _context.Members.FindAsync(id);
        if (member is null)
            return ServiceResult<MemberViewModel>.NotFound(
                $"No member found with ID {id}. It may have been deleted or never existed.");
        return ServiceResult<MemberViewModel>.Ok(MapToViewModel(member));
    }

    public async Task<ServiceResult<MemberViewModel>> CreateAsync(CreateMemberDto dto, int currentUserId)
    {
        var emailExists = await _context.Members.AnyAsync(m => m.Email == dto.Email);
        if (emailExists)
            return ServiceResult<MemberViewModel>.Conflict(
                $"A member with email '{dto.Email}' already exists in the system.");

        var member = new Member
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone,
            MembershipDate = dto.MembershipDate,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = currentUserId.ToString()
        };
        _context.Members.Add(member);
        await _context.SaveChangesAsync();
        await _log.LogAsync(currentUserId, "CreateMember", "Member", member.Id.ToString(),
            new { member.FirstName, member.LastName, member.Email });
        return ServiceResult<MemberViewModel>.Created(MapToViewModel(member), "Member created successfully.");
    }

    public async Task<ServiceResult<MemberViewModel>> UpdateAsync(int id, UpdateMemberDto dto, int currentUserId)
    {
        var member = await _context.Members.FindAsync(id);
        if (member is null)
            return ServiceResult<MemberViewModel>.NotFound(
                $"No member found with ID {id}. It may have been deleted or never existed.");

        var emailConflict = await _context.Members
            .AnyAsync(m => m.Email == dto.Email && m.Id != id);
        if (emailConflict)
            return ServiceResult<MemberViewModel>.Conflict(
                $"A member with email '{dto.Email}' already exists in the system.");

        member.FirstName = dto.FirstName;
        member.LastName = dto.LastName;
        member.Email = dto.Email;
        member.Phone = dto.Phone;
        member.IsActive = dto.IsActive;
        member.UpdatedAt = DateTime.UtcNow;
        member.UpdatedBy = currentUserId.ToString();
        await _context.SaveChangesAsync();
        await _log.LogAsync(currentUserId, "UpdateMember", "Member", id.ToString(),
            new { dto.FirstName, dto.LastName });
        return ServiceResult<MemberViewModel>.Ok(MapToViewModel(member), "Member updated successfully.");
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int id, int currentUserId)
    {
        var member = await _context.Members.FindAsync(id);
        if (member is null)
            return ServiceResult<bool>.NotFound(
                $"No member found with ID {id}. It may have been deleted or never existed.");

        member.IsDeleted = true;
        member.UpdatedAt = DateTime.UtcNow;
        member.UpdatedBy = currentUserId.ToString();
        await _context.SaveChangesAsync();
        await _log.LogAsync(currentUserId, "DeleteMember", "Member", id.ToString(), null);
        return ServiceResult<bool>.Ok(true, "Member deleted successfully.");
    }

    private static MemberViewModel MapToViewModel(Member m) => new()
    {
        Id = m.Id,
        FirstName = m.FirstName,
        LastName = m.LastName,
        Email = m.Email,
        Phone = m.Phone,
        MembershipDate = m.MembershipDate,
        IsActive = m.IsActive,
        CreatedAt = m.CreatedAt
    };
}
