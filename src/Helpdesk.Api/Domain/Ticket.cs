namespace Helpdesktracker.Api.Domain;

public class Ticket
{
    // Primary Key
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;

    // Table Data
    public string Title { get; set; } = string.Empty;
    public TicketStatus Status { get; set; }
    public TicketPriority Priority { get; set; }
    public TicketCategory Category { get; set; }

    // This is currently a Unix Time timestamp.
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ResolvedAt { get; set; }
    public DateTimeOffset? SlaDueAt { get; set; }

    // Foreign Keys
    public int RequesterId { get; set; }
    public int? AssigneeId { get; set; }

    // Navigation Properties: Not Table Columns.
    public User? Requester { get; set; }
    public User? Assignee { get; set; }
}