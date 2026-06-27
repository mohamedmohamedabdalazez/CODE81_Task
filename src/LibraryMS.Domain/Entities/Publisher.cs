namespace LibraryMS.Domain.Entities;

public class Publisher : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Website { get; set; }

    public ICollection<Book> Books { get; set; } = new List<Book>();
}
