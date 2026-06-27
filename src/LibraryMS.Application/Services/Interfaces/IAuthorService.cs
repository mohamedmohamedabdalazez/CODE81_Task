using LibraryMS.Application.Common;
using LibraryMS.Application.DTOs.Authors;
using LibraryMS.Application.ViewModels;

namespace LibraryMS.Application.Services.Interfaces;

public interface IAuthorService
{
    Task<ServiceResult<List<AuthorViewModel>>> GetAllAsync();
    Task<ServiceResult<AuthorViewModel>> GetByIdAsync(int id);
    Task<ServiceResult<AuthorViewModel>> CreateAsync(CreateAuthorDto dto, int currentUserId);
    Task<ServiceResult<AuthorViewModel>> UpdateAsync(int id, UpdateAuthorDto dto, int currentUserId);
    Task<ServiceResult<bool>> DeleteAsync(int id, int currentUserId);
}
