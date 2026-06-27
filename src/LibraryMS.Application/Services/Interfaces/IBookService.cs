using LibraryMS.Application.Common;
using LibraryMS.Application.DTOs.Books;
using LibraryMS.Application.ViewModels;
using Microsoft.AspNetCore.Http;

namespace LibraryMS.Application.Services.Interfaces;

public interface IBookService
{
    Task<ServiceResult<List<BookViewModel>>> GetAllAsync();
    Task<ServiceResult<BookViewModel>> GetByIdAsync(int id);
    Task<ServiceResult<BookViewModel>> CreateAsync(CreateBookDto dto, int currentUserId);
    Task<ServiceResult<BookViewModel>> UpdateAsync(int id, UpdateBookDto dto, int currentUserId);
    Task<ServiceResult<bool>> DeleteAsync(int id, int currentUserId);
    Task<ServiceResult<BookViewModel>> UploadCoverAsync(int bookId, IFormFile file, int currentUserId);
    Task<ServiceResult<List<BookViewModel>>> SearchAsync(string? name, string? author, string? category);
    Task<ServiceResult<List<BookViewModel>>> GetByStatusAsync(string status);
}
