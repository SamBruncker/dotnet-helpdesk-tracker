using HelpdeskTracker.Api.Domain;

namespace HelpdeskTracker.Api.Dtos;

public class TicketResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public TicketStatus Status { get; set; }

    public TicketPriority Priority { get; set; }

    public TicketCategory Category { get; set; }

    public int? AssigneeId { get; set; }

    public int RequesterId { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public DateTimeOffset? ResolvedAt { get; set; }

    public DateTimeOffset? SlaDueAt { get; set; }
}