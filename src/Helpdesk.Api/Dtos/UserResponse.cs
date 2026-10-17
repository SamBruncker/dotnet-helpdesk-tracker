using HelpdeskTracker.Api.Domain;

namespace HelpdeskTracker.Api.Dtos;

public class UserResponseDto
{
    // No data validation here because the data is coming from the database and should already be valid.
    public int Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;
}
