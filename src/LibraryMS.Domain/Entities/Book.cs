using LibraryMS.Domain.Enums;

namespace LibraryMS.Domain.Entities;

public class Book : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public string? Edition { get; set; }
    public string? Language { get; set; }
    public int? PublicationYear { get; set; }
    public string? Summary { get; set; }
    public string? CoverImagePath { get; set; }
    public BookStatus Status { get; set; } = BookStatus.Available;

    public int PublisherId { get; set; }
    public Publisher Publisher { get; set; } = null!;

    public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
    public ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();
    public ICollection<BorrowingTransaction> BorrowingTransactions { get; set; } = new List<BorrowingTransaction>();
}
