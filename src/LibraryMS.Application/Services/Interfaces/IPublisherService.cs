using LibraryMS.Application.Common;
using LibraryMS.Application.DTOs.Publishers;
using LibraryMS.Application.ViewModels;

namespace LibraryMS.Application.Services.Interfaces;

public interface IPublisherService
{
    Task<ServiceResult<List<PublisherViewModel>>> GetAllAsync();
    Task<ServiceResult<PublisherViewModel>> GetByIdAsync(int id);
    Task<ServiceResult<PublisherViewModel>> CreateAsync(CreatePublisherDto dto, int currentUserId);
    Task<ServiceResult<PublisherViewModel>> UpdateAsync(int id, UpdatePublisherDto dto, int currentUserId);
    Task<ServiceResult<bool>> DeleteAsync(int id, int currentUserId);
}
