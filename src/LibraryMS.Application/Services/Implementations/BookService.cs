using LibraryMS.Application.Common;
using LibraryMS.Application.DTOs.Books;
using LibraryMS.Application.Services.Interfaces;
using LibraryMS.Application.ViewModels;
using LibraryMS.Domain.Entities;
using LibraryMS.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace LibraryMS.Application.Services.Implementations;

public class BookService : IBookService
{
    private readonly ILibraryDbContext _context;
    private readonly IActivityLogService _log;
    private readonly string _coverBasePath;

    public BookService(ILibraryDbContext context, IActivityLogService log, IWebHostEnvironmentAccessor env)
    {
        _context = context;
        _log = log;
        _coverBasePath = Path.Combine(env.WebRootPath, "covers");
        Directory.CreateDirectory(_coverBasePath);
    }

    public async Task<ServiceResult<List<BookViewModel>>> GetAllAsync()
    {
        var books = await BuildBookQuery().ToListAsync();
        return ServiceResult<List<BookViewModel>>.Ok(books);
    }

    public async Task<ServiceResult<BookViewModel>> GetByIdAsync(int id)
    {
        var book = await BuildBookQuery().FirstOrDefaultAsync(b => b.Id == id);
        if (book is null)
            return ServiceResult<BookViewModel>.NotFound(
                $"No book found with ID {id}. It may have been deleted or never existed.");
        return ServiceResult<BookViewModel>.Ok(book);
    }

    public async Task<ServiceResult<BookViewModel>> CreateAsync(CreateBookDto dto, int currentUserId)
    {
        var isbnExists = await _context.Books.AnyAsync(b => b.ISBN == dto.ISBN);
        if (isbnExists)
            return ServiceResult<BookViewModel>.Conflict(
                $"A book with ISBN '{dto.ISBN}' already exists in the system.");

        var book = new Book
        {
            Title = dto.Title,
            ISBN = dto.ISBN,
            Edition = dto.Edition,
            Language = dto.Language,
            PublicationYear = dto.PublicationYear,
            Summary = dto.Summary,
            PublisherId = dto.PublisherId,
            Status = BookStatus.Available,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = currentUserId.ToString()
        };
        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        foreach (var authorId in dto.AuthorIds)
            _context.BookAuthors.Add(new BookAuthor { BookId = book.Id, AuthorId = authorId });
        foreach (var catId in dto.CategoryIds)
            _context.BookCategories.Add(new BookCategory { BookId = book.Id, CategoryId = catId });
        await _context.SaveChangesAsync();

        await _log.LogAsync(currentUserId, "CreateBook", "Book", book.Id.ToString(),
            new { book.Title, book.ISBN });

        var result = await GetByIdAsync(book.Id);
        return ServiceResult<BookViewModel>.Created(result.Data!, "Book created successfully.");
    }

    public async Task<ServiceResult<BookViewModel>> UpdateAsync(int id, UpdateBookDto dto, int currentUserId)
    {
        var book = await _context.Books.FindAsync(id);
        if (book is null)
            return ServiceResult<BookViewModel>.NotFound(
                $"No book found with ID {id}. It may have been deleted or never existed.");

        var isbnConflict = await _context.Books.AnyAsync(b => b.ISBN == dto.ISBN && b.Id != id);
        if (isbnConflict)
            return ServiceResult<BookViewModel>.Conflict(
                $"A book with ISBN '{dto.ISBN}' already exists in the system.");

        book.Title = dto.Title;
        book.ISBN = dto.ISBN;
        book.Edition = dto.Edition;
        book.Language = dto.Language;
        book.PublicationYear = dto.PublicationYear;
        book.Summary = dto.Summary;
        book.PublisherId = dto.PublisherId;
        book.UpdatedAt = DateTime.UtcNow;
        book.UpdatedBy = currentUserId.ToString();

        // Replace authors and categories
        var existingAuthors = _context.BookAuthors.Where(ba => ba.BookId == id);
        _context.BookAuthors.RemoveRange(existingAuthors);
        var existingCats = _context.BookCategories.Where(bc => bc.BookId == id);
        _context.BookCategories.RemoveRange(existingCats);

        foreach (var authorId in dto.AuthorIds)
            _context.BookAuthors.Add(new BookAuthor { BookId = id, AuthorId = authorId });
        foreach (var catId in dto.CategoryIds)
            _context.BookCategories.Add(new BookCategory { BookId = id, CategoryId = catId });

        await _context.SaveChangesAsync();
        await _log.LogAsync(currentUserId, "UpdateBook", "Book", id.ToString(),
            new { dto.Title, dto.ISBN });

        var result = await GetByIdAsync(id);
        return ServiceResult<BookViewModel>.Ok(result.Data!, "Book updated successfully.");
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int id, int currentUserId)
    {
        var book = await _context.Books.FindAsync(id);
        if (book is null)
            return ServiceResult<bool>.NotFound(
                $"No book found with ID {id}. It may have been deleted or never existed.");

        book.IsDeleted = true;
        book.UpdatedAt = DateTime.UtcNow;
        book.UpdatedBy = currentUserId.ToString();
        await _context.SaveChangesAsync();
        await _log.LogAsync(currentUserId, "DeleteBook", "Book", id.ToString(), null);
        return ServiceResult<bool>.Ok(true, "Book deleted successfully.");
    }

    public async Task<ServiceResult<BookViewModel>> UploadCoverAsync(int bookId, IFormFile file, int currentUserId)
    {
        var book = await _context.Books.FindAsync(bookId);
        if (book is null)
            return ServiceResult<BookViewModel>.NotFound(
                $"No book found with ID {bookId}. It may have been deleted or never existed.");

        var ext = Path.GetExtension(file.FileName);
        var fileName = $"{bookId}_{Guid.NewGuid():N}{ext}";
        var fullPath = Path.Combine(_coverBasePath, fileName);
        await using var stream = File.Create(fullPath);
        await file.CopyToAsync(stream);

        book.CoverImagePath = $"/covers/{fileName}";
        book.UpdatedAt = DateTime.UtcNow;
        book.UpdatedBy = currentUserId.ToString();
        await _context.SaveChangesAsync();
        await _log.LogAsync(currentUserId, "UploadCover", "Book", bookId.ToString(),
            new { book.CoverImagePath });

        var result = await GetByIdAsync(bookId);
        return ServiceResult<BookViewModel>.Ok(result.Data!, "Cover image uploaded successfully.");
    }

    public async Task<ServiceResult<List<BookViewModel>>> SearchAsync(string? name, string? author, string? category)
    {
        var query = _context.Books
            .Include(b => b.Publisher)
            .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
            .Include(b => b.BookCategories).ThenInclude(bc => bc.Category)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(b => b.Title.Contains(name));

        if (!string.IsNullOrWhiteSpace(author))
            query = query.Where(b => b.BookAuthors.Any(ba =>
                (ba.Author.FirstName + " " + ba.Author.LastName).Contains(author) ||
                ba.Author.FirstName.Contains(author) ||
                ba.Author.LastName.Contains(author)));

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(b => b.BookCategories.Any(bc =>
                bc.Category.Name.Contains(category)));

        var books = await query.Select(b => ProjectToViewModel(b)).ToListAsync();
        return ServiceResult<List<BookViewModel>>.Ok(books);
    }

    public async Task<ServiceResult<List<BookViewModel>>> GetByStatusAsync(string status)
    {
        if (!Enum.TryParse<BookStatus>(status, true, out var bookStatus))
            return ServiceResult<List<BookViewModel>>.Fail(
                $"Invalid status '{status}'. Valid values are: Available, Borrowed.");

        var books = await _context.Books
            .Include(b => b.Publisher)
            .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
            .Include(b => b.BookCategories).ThenInclude(bc => bc.Category)
            .Where(b => b.Status == bookStatus)
            .Select(b => ProjectToViewModel(b))
            .ToListAsync();
        return ServiceResult<List<BookViewModel>>.Ok(books);
    }

    private IQueryable<BookViewModel> BuildBookQuery() =>
        _context.Books
            .Include(b => b.Publisher)
            .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
            .Include(b => b.BookCategories).ThenInclude(bc => bc.Category)
            .Select(b => ProjectToViewModel(b));

    private static BookViewModel ProjectToViewModel(Book b) => new()
    {
        Id = b.Id,
        Title = b.Title,
        ISBN = b.ISBN,
        Edition = b.Edition,
        Language = b.Language,
        PublicationYear = b.PublicationYear,
        Summary = b.Summary,
        CoverImagePath = b.CoverImagePath,
        Status = b.Status.ToString(),
        PublisherName = b.Publisher.Name,
        AuthorNames = b.BookAuthors.Select(ba => ba.Author.FirstName + " " + ba.Author.LastName).ToList(),
        CategoryNames = b.BookCategories.Select(bc => bc.Category.Name).ToList(),
        CreatedAt = b.CreatedAt
    };
}
