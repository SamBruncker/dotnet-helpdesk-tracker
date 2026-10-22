using System.ComponentModel.DataAnnotations;
using HelpdeskTracker.Api.Domain;

namespace HelpdeskTracker.Api.Dtos;

public class AssignTicketRequestDto
{

    public required int? AssigneeId { get; set; }
}