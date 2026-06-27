namespace LibraryMS.Application.ViewModels;

public class AuthorViewModel
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string? Bio { get; set; }
    public DateTime CreatedAt { get; set; }
}
