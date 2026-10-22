using System.ComponentModel.DataAnnotations;
using HelpdeskTracker.Api.Domain;

namespace HelpdeskTracker.Api.Dtos;

public class UpdateTicketRequestDto
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(2500)]
    public string Description { get; set; } = string.Empty;

    [EnumDataType(typeof(TicketPriority))]
    public required TicketPriority Priority { get; set; }

    [EnumDataType(typeof(TicketCategory))]
    public required TicketCategory Category { get; set; }
}