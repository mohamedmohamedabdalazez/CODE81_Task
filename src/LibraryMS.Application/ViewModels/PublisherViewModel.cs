namespace LibraryMS.Application.ViewModels;

public class PublisherViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Website { get; set; }
    public DateTime CreatedAt { get; set; }
}
