using HelpdeskTracker.Api.Domain;
using HelpdeskTracker.Api.Services;
using HelpdeskTracker.Api.Dtos;
using HelpdeskTracker.Api.Services.Results;
using Microsoft.AspNetCore.Mvc;

namespace HelpdeskTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _ticketService;

    public TicketsController(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TicketResponseDto>>> GetAll(TicketStatus? status, int? assigneeId, CancellationToken cancellationToken)
    {
        var tickets = await _ticketService.GetAllAsync(
            status: status,
            assigneeId: assigneeId,
            cancellationToken: cancellationToken);

        return Ok(tickets);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TicketResponseDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _ticketService.GetByIdAsync(id, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.Error switch
            {
                ErrorKind.NotFound => NotFound(result.ErrorMessage),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result.ErrorMessage)
            };
        }

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<ActionResult<TicketResponseDto>> Create([FromBody] CreateTicketRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _ticketService.CreateAsync(request, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.Error switch
            {
                ErrorKind.Validation => BadRequest(result.ErrorMessage),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result.ErrorMessage)
            };
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TicketResponseDto>> Update(int id, [FromBody] UpdateTicketRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _ticketService.UpdateAsync(id, request, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.Error switch
            {
                ErrorKind.NotFound => NotFound(result.ErrorMessage),
                ErrorKind.Conflict => Conflict(result.ErrorMessage),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result.ErrorMessage)
            };
        }

        return Ok(result.Value);
    }

    [HttpPost("{id:int}/assign")]
    public async Task<ActionResult<TicketResponseDto>> Assign(int id, [FromBody] AssignTicketRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _ticketService.AssignAsync(id, request, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.Error switch
            {
                ErrorKind.NotFound => NotFound(result.ErrorMessage),
                ErrorKind.Conflict => Conflict(result.ErrorMessage),
                ErrorKind.Validation => BadRequest(result.ErrorMessage),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result.ErrorMessage)
            };
        }
        return Ok(result.Value);
    }

    [HttpPost("{id:int}/status")]
    public async Task<ActionResult<TicketResponseDto>> ChangeStatus(int id, [FromBody] ChangeTicketStatusRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _ticketService.ChangeStatusAsync(id, request, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.Error switch
            {
                ErrorKind.NotFound => NotFound(result.ErrorMessage),
                ErrorKind.Conflict => Conflict(result.ErrorMessage),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result.ErrorMessage)
            };
        }

        return Ok(result.Value);
    }

}