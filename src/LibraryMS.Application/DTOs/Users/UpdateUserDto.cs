using LibraryMS.Domain.Enums;

namespace LibraryMS.Application.DTOs.Users;

public class UpdateUserDto
{
    public string? Password { get; set; }
    public UserRole Role { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
