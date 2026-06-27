namespace LibraryMS.Application.DTOs.Books;

public class UpdateBookDto
{
    public string Title { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public string? Edition { get; set; }
    public string? Language { get; set; }
    public int? PublicationYear { get; set; }
    public string? Summary { get; set; }
    public int PublisherId { get; set; }
    public List<int> AuthorIds { get; set; } = new();
    public List<int> CategoryIds { get; set; } = new();
}
