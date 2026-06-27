using LibraryMS.Application.Common;
using LibraryMS.Application.DTOs.Members;
using LibraryMS.Application.ViewModels;

namespace LibraryMS.Application.Services.Interfaces;

public interface IMemberService
{
    Task<ServiceResult<List<MemberViewModel>>> GetAllAsync();
    Task<ServiceResult<MemberViewModel>> GetByIdAsync(int id);
    Task<ServiceResult<MemberViewModel>> CreateAsync(CreateMemberDto dto, int currentUserId);
    Task<ServiceResult<MemberViewModel>> UpdateAsync(int id, UpdateMemberDto dto, int currentUserId);
    Task<ServiceResult<bool>> DeleteAsync(int id, int currentUserId);
}
