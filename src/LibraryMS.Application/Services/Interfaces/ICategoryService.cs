using LibraryMS.Application.Common;
using LibraryMS.Application.DTOs.Categories;
using LibraryMS.Application.ViewModels;

namespace LibraryMS.Application.Services.Interfaces;

public interface ICategoryService
{
    Task<ServiceResult<List<CategoryViewModel>>> GetAllAsync();
    Task<ServiceResult<CategoryViewModel>> GetByIdAsync(int id);
    Task<ServiceResult<CategoryViewModel>> CreateAsync(CreateCategoryDto dto, int currentUserId);
    Task<ServiceResult<CategoryViewModel>> UpdateAsync(int id, UpdateCategoryDto dto, int currentUserId);
    Task<ServiceResult<bool>> DeleteAsync(int id, int currentUserId);
}
