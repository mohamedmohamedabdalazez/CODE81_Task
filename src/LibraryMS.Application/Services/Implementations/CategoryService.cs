using LibraryMS.Application.Common;
using LibraryMS.Application.DTOs.Categories;
using LibraryMS.Application.Services.Interfaces;
using LibraryMS.Application.ViewModels;
using LibraryMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryMS.Application.Services.Implementations;

public class CategoryService : ICategoryService
{
    private readonly ILibraryDbContext _context;
    private readonly IActivityLogService _log;

    public CategoryService(ILibraryDbContext context, IActivityLogService log)
    {
        _context = context;
        _log = log;
    }

    public async Task<ServiceResult<List<CategoryViewModel>>> GetAllAsync()
    {
        var categories = await _context.Categories
            .Include(c => c.SubCategories)
            .Include(c => c.ParentCategory)
            .Where(c => c.ParentCategoryId == null)
            .ToListAsync();

        var viewModels = categories.Select(c => MapToViewModel(c)).ToList();
        return ServiceResult<List<CategoryViewModel>>.Ok(viewModels);
    }

    public async Task<ServiceResult<CategoryViewModel>> GetByIdAsync(int id)
    {
        var category = await _context.Categories
            .Include(c => c.SubCategories)
            .Include(c => c.ParentCategory)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category is null)
            return ServiceResult<CategoryViewModel>.NotFound(
                $"No category found with ID {id}. It may have been deleted or never existed.");
        return ServiceResult<CategoryViewModel>.Ok(MapToViewModel(category));
    }

    public async Task<ServiceResult<CategoryViewModel>> CreateAsync(CreateCategoryDto dto, int currentUserId)
    {
        if (dto.ParentCategoryId.HasValue)
        {
            var parent = await _context.Categories.FindAsync(dto.ParentCategoryId.Value);
            if (parent is null)
                return ServiceResult<CategoryViewModel>.NotFound(
                    $"No category found with ID {dto.ParentCategoryId}. It may have been deleted or never existed.");

            // Enforce two-level limit: parent itself must be a top-level category
            if (parent.ParentCategoryId.HasValue)
                return ServiceResult<CategoryViewModel>.Fail(
                    $"Category '{parent.Name}' is already a child category and cannot be used as a parent. Only two levels of hierarchy are allowed.");
        }

        var category = new Category
        {
            Name = dto.Name,
            ParentCategoryId = dto.ParentCategoryId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = currentUserId.ToString()
        };
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        await _log.LogAsync(currentUserId, "CreateCategory", "Category", category.Id.ToString(),
            new { category.Name, category.ParentCategoryId });

        var result = await GetByIdAsync(category.Id);
        return ServiceResult<CategoryViewModel>.Created(result.Data!, "Category created successfully.");
    }

    public async Task<ServiceResult<CategoryViewModel>> UpdateAsync(int id, UpdateCategoryDto dto, int currentUserId)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category is null)
            return ServiceResult<CategoryViewModel>.NotFound(
                $"No category found with ID {id}. It may have been deleted or never existed.");

        if (dto.ParentCategoryId.HasValue)
        {
            var parent = await _context.Categories.FindAsync(dto.ParentCategoryId.Value);
            if (parent is null)
                return ServiceResult<CategoryViewModel>.NotFound(
                    $"No category found with ID {dto.ParentCategoryId}. It may have been deleted or never existed.");

            if (parent.ParentCategoryId.HasValue)
                return ServiceResult<CategoryViewModel>.Fail(
                    $"Category '{parent.Name}' is already a child category and cannot be used as a parent. Only two levels of hierarchy are allowed.");
        }

        category.Name = dto.Name;
        category.ParentCategoryId = dto.ParentCategoryId;
        category.UpdatedAt = DateTime.UtcNow;
        category.UpdatedBy = currentUserId.ToString();
        await _context.SaveChangesAsync();
        await _log.LogAsync(currentUserId, "UpdateCategory", "Category", id.ToString(), new { dto.Name });

        var result = await GetByIdAsync(id);
        return ServiceResult<CategoryViewModel>.Ok(result.Data!, "Category updated successfully.");
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int id, int currentUserId)
    {
        var category = await _context.Categories
            .Include(c => c.SubCategories)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category is null)
            return ServiceResult<bool>.NotFound(
                $"No category found with ID {id}. It may have been deleted or never existed.");

        if (category.SubCategories.Any())
            return ServiceResult<bool>.Fail(
                $"Cannot delete category '{category.Name}' because it has {category.SubCategories.Count} child categories. Remove or reassign them first.");

        category.IsDeleted = true;
        category.UpdatedAt = DateTime.UtcNow;
        category.UpdatedBy = currentUserId.ToString();
        await _context.SaveChangesAsync();
        await _log.LogAsync(currentUserId, "DeleteCategory", "Category", id.ToString(), null);
        return ServiceResult<bool>.Ok(true, "Category deleted successfully.");
    }

    private static CategoryViewModel MapToViewModel(Category c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        ParentCategoryId = c.ParentCategoryId,
        ParentCategoryName = c.ParentCategory?.Name,
        SubCategories = c.SubCategories
            .Where(s => !s.IsDeleted)
            .Select(s => new CategoryViewModel { Id = s.Id, Name = s.Name, ParentCategoryId = s.ParentCategoryId })
            .ToList(),
        CreatedAt = c.CreatedAt
    };
}
