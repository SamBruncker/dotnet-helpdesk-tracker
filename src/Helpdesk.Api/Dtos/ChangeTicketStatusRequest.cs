using System.ComponentModel.DataAnnotations;
using HelpdeskTracker.Api.Domain;

namespace HelpdeskTracker.Api.Dtos;

public class ChangeTicketStatusRequestDto
{
    [EnumDataType(typeof(TicketStatus))]
    public required TicketStatus Status { get; set; }
}