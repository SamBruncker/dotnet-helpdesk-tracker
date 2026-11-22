namespace HelpdeskTracker.Api.Domain;

public static class TicketTransitions
{
    private static readonly Dictionary<TicketStatus, List<TicketStatus>> _transitions = new()
    {
        { TicketStatus.New, new List<TicketStatus> { TicketStatus.InProgress, TicketStatus.Resolved, TicketStatus.Unresolved } },
        { TicketStatus.InProgress, new List<TicketStatus> { TicketStatus.OnHold, TicketStatus.Resolved, TicketStatus.Unresolved } },
        { TicketStatus.OnHold, new List<TicketStatus> { TicketStatus.InProgress, TicketStatus.Unresolved}},
        { TicketStatus.Resolved, new List<TicketStatus> {}},
        { TicketStatus.Unresolved, new List<TicketStatus> {}}
    };

    public static bool CanTransition(TicketStatus fromStatus, TicketStatus toStatus)
    {
        return _transitions.TryGetValue(fromStatus, out var allowedTransitions) && allowedTransitions.Contains(toStatus);  
    }
}