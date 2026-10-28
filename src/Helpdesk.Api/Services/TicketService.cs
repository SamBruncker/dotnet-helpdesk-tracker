using Microsoft.EntityFrameworkCore;
using HelpdeskTracker.Api.Data;
using HelpdeskTracker.Api.Domain;
using HelpdeskTracker.Api.Dtos;
using HelpdeskTracker.Api.Services.Results;
using Microsoft.Data.SqlClient;

namespace HelpdeskTracker.Api.Services;

public class TicketService : ITicketService
{
    private readonly HelpdeskDbContext _dbContext;

    public TicketService(HelpdeskDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<TicketResponseDto>> GetAllAsync(TicketStatus? status = null, int? assigneeId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Tickets.AsNoTracking();

        if (status != null)
        {
            query = query.Where(t => t.Status == status);
        }
        if (assigneeId != null)
        {
            query = query.Where(t => t.AssigneeId == assigneeId);
        }

        var tickets = await query
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new TicketResponseDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Priority = t.Priority,
                Category = t.Category,
                RequesterId = t.RequesterId,
                Status = t.Status,
                AssigneeId = t.AssigneeId,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                ResolvedAt = t.ResolvedAt,
                SlaDueAt = t.SlaDueAt
            })
            .ToListAsync(cancellationToken);

        return tickets;
    }
    public async Task<ServiceResult<TicketResponseDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var ticket = await _dbContext.Tickets.AsNoTracking()
            .Where(t => t.Id == id)
            .Select(t => new TicketResponseDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Priority = t.Priority,
                Category = t.Category,
                RequesterId = t.RequesterId,
                Status = t.Status,
                AssigneeId = t.AssigneeId,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                ResolvedAt = t.ResolvedAt,
                SlaDueAt = t.SlaDueAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (ticket is null)
        {
            return ServiceResult<TicketResponseDto>.Fail(ErrorKind.NotFound, "Ticket not found.");
        }
        return ServiceResult<TicketResponseDto>.Ok(ticket);
    }
    public async Task<ServiceResult<TicketResponseDto>> CreateAsync(CreateTicketRequestDto request, CancellationToken cancellationToken = default)
    {
        string ticketTitle = request.Title.Trim();
        string ticketDescription = request.Description.Trim();

        // No Check for duplicate here. Identical tickets can be raised. There are legitimate business cases for this.

        var requester = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == request.RequesterId, cancellationToken);

        if (requester is null)
        {
            return ServiceResult<TicketResponseDto>.Fail(ErrorKind.Validation, "Requester not found.");
        }
        if (!requester.IsActive)
        {
            return ServiceResult<TicketResponseDto>.Fail(ErrorKind.Validation, "Deactivated users cannot create tickets.");
        }

        Ticket ticket = new Ticket
        {
            Title = ticketTitle,
            Description = ticketDescription,
            Priority = request.Priority,
            Category = request.Category,
            Status = TicketStatus.New,
            RequesterId = request.RequesterId
        };

        try
        {
            _dbContext.Tickets.Add(ticket);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 547 })
        {
            return ServiceResult<TicketResponseDto>.Fail(
                ErrorKind.Validation,
                "Requester no longer available.");
        }

        return ServiceResult<TicketResponseDto>.Ok(MapToTicketResponseDto(ticket));

    }
    public async Task<ServiceResult<TicketResponseDto>> UpdateAsync(int id, UpdateTicketRequestDto request, CancellationToken cancellationToken = default)
    {
        var ticket = await _dbContext.Tickets.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        if (ticket is null)
        {
            return ServiceResult<TicketResponseDto>.Fail(ErrorKind.NotFound, "Ticket not found.");
        }

        if (ticket.Status is TicketStatus.Resolved || ticket.Status is TicketStatus.Unresolved)
        {
            return ServiceResult<TicketResponseDto>.Fail(ErrorKind.Conflict, "This ticket has been closed and cannot be edited.");
        }

        string title = request.Title.Trim();
        string description = request.Description.Trim();

        ticket.Title = title;
        ticket.Description = description;
        ticket.Priority = request.Priority;
        ticket.Category = request.Category;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return ServiceResult<TicketResponseDto>.Ok(MapToTicketResponseDto(ticket));

    }
    public async Task<ServiceResult<TicketResponseDto>> AssignAsync(int id, AssignTicketRequestDto request, CancellationToken cancellationToken = default)
    {
        var ticket = await _dbContext.Tickets.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        if (ticket is null)
        {
            return ServiceResult<TicketResponseDto>.Fail(ErrorKind.NotFound, "Ticket not found.");
        }

        if (ticket.Status is TicketStatus.Resolved || ticket.Status is TicketStatus.Unresolved)
        {
            return ServiceResult<TicketResponseDto>.Fail(ErrorKind.Conflict, "This ticket has been closed and cannot be edited.");
        }

        if (request.AssigneeId is not null)
        {
            var user = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == request.AssigneeId, cancellationToken);

            // validate the user.
            if (user is null)
            {
                return ServiceResult<TicketResponseDto>.Fail(ErrorKind.Validation, "Ticket Assignee not found.");
            }
            if (!user.IsActive)
            {
                return ServiceResult<TicketResponseDto>.Fail(ErrorKind.Validation, "Tickets cannot be assigned to deactivated users.");
            }
            if (user.Role is not UserRole.Agent)
            {
                return ServiceResult<TicketResponseDto>.Fail(ErrorKind.Validation, "Tickets can only be assigned to agents.");
            }
        }

        ticket.AssigneeId = request.AssigneeId;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return ServiceResult<TicketResponseDto>.Ok(MapToTicketResponseDto(ticket));
    }
    public async Task<ServiceResult<TicketResponseDto>> ChangeStatusAsync(int id, ChangeTicketStatusRequestDto request, CancellationToken cancellationToken = default)
    {
        var ticket = await _dbContext.Tickets.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        if (ticket is null)
        {
            return ServiceResult<TicketResponseDto>.Fail(ErrorKind.NotFound, "Ticket not found.");
        }

        if (ticket.Status is TicketStatus.Resolved || ticket.Status is TicketStatus.Unresolved)
        {
            return ServiceResult<TicketResponseDto>.Fail(ErrorKind.Conflict, "The status of closed tickets cannot be changed.");
        }

        if (request.Status == TicketStatus.Resolved)
        {
            ticket.ResolvedAt = DateTimeOffset.UtcNow;
        }

        ticket.Status = request.Status;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return ServiceResult<TicketResponseDto>.Ok(MapToTicketResponseDto(ticket));
    }

    private static TicketResponseDto MapToTicketResponseDto(Ticket ticket)
    {
        return new TicketResponseDto
        {
            Id = ticket.Id,
            Title = ticket.Title,
            Description = ticket.Description,
            Priority = ticket.Priority,
            Category = ticket.Category,
            RequesterId = ticket.RequesterId,
            Status = ticket.Status,
            AssigneeId = ticket.AssigneeId,
            CreatedAt = ticket.CreatedAt,
            UpdatedAt = ticket.UpdatedAt,
            ResolvedAt = ticket.ResolvedAt,
            SlaDueAt = ticket.SlaDueAt
        };
    }
}