using LibraryMS.Application.Common;
using LibraryMS.Application.DTOs.Borrowing;
using LibraryMS.Application.Services.Interfaces;
using LibraryMS.Application.ViewModels;
using LibraryMS.Domain.Entities;
using LibraryMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LibraryMS.Application.Services.Implementations;

public class BorrowingService : IBorrowingService
{
    private const int MaxActiveBorrows = 5;
    private const int LoanDays = 14;

    private readonly ILibraryDbContext _context;
    private readonly IActivityLogService _log;

    public BorrowingService(ILibraryDbContext context, IActivityLogService log)
    {
        _context = context;
        _log = log;
    }

    public async Task<ServiceResult<BorrowingTransactionViewModel>> BorrowAsync(BorrowBookDto dto, int currentUserId)
    {
        var book = await _context.Books.FindAsync(dto.BookId);
        if (book is null)
            return ServiceResult<BorrowingTransactionViewModel>.NotFound(
                $"No book found with ID {dto.BookId}. It may have been deleted or never existed.");

        if (book.Status == BookStatus.Borrowed)
            return ServiceResult<BorrowingTransactionViewModel>.Fail(
                $"Book '{book.Title}' (ISBN: {book.ISBN}) is currently checked out and cannot be borrowed again until it is returned.");

        var member = await _context.Members.FindAsync(dto.MemberId);
        if (member is null)
            return ServiceResult<BorrowingTransactionViewModel>.NotFound(
                $"No member found with ID {dto.MemberId}. It may have been deleted or never existed.");

        var activeBorrows = await _context.BorrowingTransactions
            .CountAsync(t => t.MemberId == dto.MemberId && t.Status == BorrowStatus.Active);
        if (activeBorrows >= MaxActiveBorrows)
            return ServiceResult<BorrowingTransactionViewModel>.Fail(
                $"Member '{member.FirstName} {member.LastName}' has reached the maximum of {MaxActiveBorrows} active borrows. A book must be returned before a new one can be borrowed.");

        var now = DateTime.UtcNow;
        var transaction = new BorrowingTransaction
        {
            BookId = dto.BookId,
            MemberId = dto.MemberId,
            BorrowedByUserId = currentUserId,
            BorrowDate = now,
            DueDate = now.AddDays(LoanDays),
            Status = BorrowStatus.Active,
            CreatedAt = now,
            CreatedBy = currentUserId.ToString()
        };
        book.Status = BookStatus.Borrowed;
        _context.BorrowingTransactions.Add(transaction);
        await _context.SaveChangesAsync();
        await _log.LogAsync(currentUserId, "BorrowBook", "BorrowingTransaction",
            transaction.Id.ToString(),
            new { transaction.BookId, transaction.MemberId, DueDate = transaction.DueDate });

        return ServiceResult<BorrowingTransactionViewModel>.Created(
            await LoadTransactionViewModel(transaction.Id),
            $"Book '{book.Title}' borrowed successfully. Due date: {transaction.DueDate:yyyy-MM-dd}.");
    }

    public async Task<ServiceResult<BorrowingTransactionViewModel>> ReturnAsync(int transactionId, int currentUserId)
    {
        var transaction = await _context.BorrowingTransactions
            .Include(t => t.Book)
            .FirstOrDefaultAsync(t => t.Id == transactionId);

        if (transaction is null)
            return ServiceResult<BorrowingTransactionViewModel>.NotFound(
                $"No transaction found with ID {transactionId}. It may have been deleted or never existed.");

        if (transaction.Status == BorrowStatus.Returned)
            return ServiceResult<BorrowingTransactionViewModel>.Fail(
                $"Transaction {transactionId} has already been closed. This book was returned on {transaction.ReturnDate:yyyy-MM-dd}.");

        transaction.ReturnDate = DateTime.UtcNow;
        transaction.Status = BorrowStatus.Returned;
        transaction.ReturnedByUserId = currentUserId;
        transaction.UpdatedAt = DateTime.UtcNow;
        transaction.UpdatedBy = currentUserId.ToString();
        transaction.Book.Status = BookStatus.Available;
        await _context.SaveChangesAsync();
        await _log.LogAsync(currentUserId, "ReturnBook", "BorrowingTransaction",
            transactionId.ToString(),
            new { transactionId, transaction.BookId, ReturnDate = transaction.ReturnDate });

        return ServiceResult<BorrowingTransactionViewModel>.Ok(
            await LoadTransactionViewModel(transactionId),
            "Book returned successfully.");
    }

    public async Task<ServiceResult<List<BorrowingTransactionViewModel>>> GetAllTransactionsAsync()
    {
        var transactions = await LoadAllTransactionViewModels().ToListAsync();
        return ServiceResult<List<BorrowingTransactionViewModel>>.Ok(transactions);
    }

    public async Task<ServiceResult<List<BorrowingTransactionViewModel>>> GetByMemberAsync(int memberId)
    {
        var member = await _context.Members.FindAsync(memberId);
        if (member is null)
            return ServiceResult<List<BorrowingTransactionViewModel>>.NotFound(
                $"No member found with ID {memberId}. It may have been deleted or never existed.");

        var result = await _context.BorrowingTransactions
            .Where(t => t.MemberId == memberId)
            .Include(t => t.Book)
            .Include(t => t.Member)
            .Include(t => t.BorrowedByUser)
            .Include(t => t.ReturnedByUser)
            .Select(t => MapToViewModel(t))
            .ToListAsync();

        return ServiceResult<List<BorrowingTransactionViewModel>>.Ok(result);
    }

    private IQueryable<BorrowingTransactionViewModel> LoadAllTransactionViewModels() =>
        _context.BorrowingTransactions
            .Include(t => t.Book)
            .Include(t => t.Member)
            .Include(t => t.BorrowedByUser)
            .Include(t => t.ReturnedByUser)
            .Select(t => MapToViewModel(t));

    private async Task<BorrowingTransactionViewModel> LoadTransactionViewModel(int id) =>
        await _context.BorrowingTransactions
            .Include(t => t.Book)
            .Include(t => t.Member)
            .Include(t => t.BorrowedByUser)
            .Include(t => t.ReturnedByUser)
            .Where(t => t.Id == id)
            .Select(t => MapToViewModel(t))
            .FirstAsync();

    private static BorrowingTransactionViewModel MapToViewModel(BorrowingTransaction t) => new()
    {
        Id = t.Id,
        BookTitle = t.Book.Title,
        BookISBN = t.Book.ISBN,
        MemberName = t.Member.FirstName + " " + t.Member.LastName,
        BorrowedByUser = t.BorrowedByUser.FullName,
        ReturnedByUser = t.ReturnedByUser != null ? t.ReturnedByUser.FullName : null,
        BorrowDate = t.BorrowDate,
        DueDate = t.DueDate,
        ReturnDate = t.ReturnDate,
        Status = t.Status.ToString()
    };
}
