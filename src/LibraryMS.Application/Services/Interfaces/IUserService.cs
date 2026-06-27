using LibraryMS.Application.Common;
using LibraryMS.Application.DTOs.Users;
using LibraryMS.Application.ViewModels;
using LibraryMS.Domain.Entities;

namespace LibraryMS.Application.Services.Interfaces;

public interface IUserService
{
    Task<ServiceResult<List<UserViewModel>>> GetAllAsync();
    Task<ServiceResult<UserViewModel>> GetByIdAsync(int id);
    Task<ServiceResult<UserViewModel>> CreateAsync(CreateUserDto dto, int currentUserId);
    Task<ServiceResult<UserViewModel>> UpdateAsync(int id, UpdateUserDto dto, int currentUserId);
    Task<ServiceResult<bool>> DeleteAsync(int id, int currentUserId);
    Task<SystemUser?> AuthenticateAsync(string username, string password);
}
