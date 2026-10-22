using HelpdeskTracker.Api.Domain;
using HelpdeskTracker.Api.Dtos;
using HelpdeskTracker.Api.Services.Results;

namespace HelpdeskTracker.Api.Services;

public interface ITicketService
{
    Task<IReadOnlyList<TicketResponseDto>> GetAllAsync(TicketStatus? status = null, int? assigneeId = null, CancellationToken cancellationToken = default);
    Task<ServiceResult<TicketResponseDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ServiceResult<TicketResponseDto>> CreateAsync(CreateTicketRequestDto request, CancellationToken cancellationToken = default);
    Task<ServiceResult<TicketResponseDto>> UpdateAsync(int id, UpdateTicketRequestDto request, CancellationToken cancellationToken = default);
    Task<ServiceResult<TicketResponseDto>> AssignAsync(int id, AssignTicketRequestDto request, CancellationToken cancellationToken = default);
    Task<ServiceResult<TicketResponseDto>> ChangeStatusAsync(int id, ChangeTicketStatusRequestDto request, CancellationToken cancellationToken = default);

}