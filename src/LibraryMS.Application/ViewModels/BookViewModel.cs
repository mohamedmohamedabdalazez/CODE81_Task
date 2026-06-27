using LibraryMS.Domain.Enums;

namespace LibraryMS.Application.ViewModels;

public class BookViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public string? Edition { get; set; }
    public string? Language { get; set; }
    public int? PublicationYear { get; set; }
    public string? Summary { get; set; }
    public string? CoverImagePath { get; set; }
    public string Status { get; set; } = string.Empty;
    public string PublisherName { get; set; } = string.Empty;
    public List<string> AuthorNames { get; set; } = new();
    public List<string> CategoryNames { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}
