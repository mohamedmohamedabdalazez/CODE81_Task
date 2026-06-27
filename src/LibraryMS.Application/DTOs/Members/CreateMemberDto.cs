namespace LibraryMS.Application.DTOs.Members;

public class CreateMemberDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public DateTime MembershipDate { get; set; } = DateTime.UtcNow;
}
