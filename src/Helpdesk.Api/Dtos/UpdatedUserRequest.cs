using System.ComponentModel.DataAnnotations;
using HelpdeskTracker.Api.Domain;

namespace HelpdeskTracker.Api.Dtos;

public class UpdateUserRequestDto
{
    [Required]
    [StringLength(100)]
    public string DisplayName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(254)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [EnumDataType(typeof(UserRole))]
    public UserRole Role { get; set; }
}