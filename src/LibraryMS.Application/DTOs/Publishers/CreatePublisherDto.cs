namespace LibraryMS.Application.DTOs.Publishers;

public class CreatePublisherDto
{
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Website { get; set; }
}
