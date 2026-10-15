namespace HelpdeskTracker.Api.Domain;

public enum TicketStatus
{
    New,
    InProgress,
    OnHold,
    Resolved,
    // Unresolved occurs when a ticket is closed without a resolution,
    // typically due to the issue being invalid, duplicate, or not reproducible.
    Unresolved
}