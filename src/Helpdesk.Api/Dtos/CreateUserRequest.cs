using System.ComponentModel.DataAnnotations;
using HelpdeskTracker.Api.Domain;

namespace HelpdeskTracker.Api.Dtos;

public class CreateUserRequestDto
{
    [Required]
    [StringLength(100)]
    public string DisplayName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(254)]
    public string Email { get; set; } = string.Empty;

    [EnumDataType(typeof(UserRole))]
    public required UserRole Role { get; set; }
}
