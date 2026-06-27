using LibraryMS.Application.Common;
using LibraryMS.Application.DTOs.Borrowing;
using LibraryMS.Application.ViewModels;

namespace LibraryMS.Application.Services.Interfaces;

public interface IBorrowingService
{
    Task<ServiceResult<BorrowingTransactionViewModel>> BorrowAsync(BorrowBookDto dto, int currentUserId);
    Task<ServiceResult<BorrowingTransactionViewModel>> ReturnAsync(int transactionId, int currentUserId);
    Task<ServiceResult<List<BorrowingTransactionViewModel>>> GetAllTransactionsAsync();
    Task<ServiceResult<List<BorrowingTransactionViewModel>>> GetByMemberAsync(int memberId);
}
