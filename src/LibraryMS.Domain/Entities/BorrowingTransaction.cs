using LibraryMS.Domain.Enums;

namespace LibraryMS.Domain.Entities;

public class BorrowingTransaction : BaseEntity
{
    public int BookId { get; set; }
    public Book Book { get; set; } = null!;

    public int MemberId { get; set; }
    public Member Member { get; set; } = null!;

    public int BorrowedByUserId { get; set; }
    public SystemUser BorrowedByUser { get; set; } = null!;

    public int? ReturnedByUserId { get; set; }
    public SystemUser? ReturnedByUser { get; set; }

    public DateTime BorrowDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public BorrowStatus Status { get; set; } = BorrowStatus.Active;
}
