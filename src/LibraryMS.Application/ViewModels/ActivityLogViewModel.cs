namespace LibraryMS.Application.ViewModels;

public class ActivityLogViewModel
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string? EntityId { get; set; }
    public DateTime Timestamp { get; set; }
    public string? Details { get; set; }
}
