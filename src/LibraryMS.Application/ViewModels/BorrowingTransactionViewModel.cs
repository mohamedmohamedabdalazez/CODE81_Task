namespace LibraryMS.Application.ViewModels;

public class BorrowingTransactionViewModel
{
    public int Id { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public string BookISBN { get; set; } = string.Empty;
    public string MemberName { get; set; } = string.Empty;
    public string BorrowedByUser { get; set; } = string.Empty;
    public string? ReturnedByUser { get; set; }
    public DateTime BorrowDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public string Status { get; set; } = string.Empty;
}
