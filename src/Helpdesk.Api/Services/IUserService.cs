using HelpdeskTracker.Api.Dtos;
using HelpdeskTracker.Api.Services.Results;

namespace HelpdeskTracker.Api.Services;

public interface IUserService
{
    Task<IReadOnlyList<UserResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<ServiceResult<UserResponseDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<ServiceResult<UserResponseDto>> CreateAsync(CreateUserRequestDto request, CancellationToken cancellationToken = default);

    Task<ServiceResult<UserResponseDto>> UpdateAsync(int id, UpdateUserRequestDto request, CancellationToken cancellationToken = default);

    Task<ServiceResult<UserResponseDto>> DeactivateAsync(int id, CancellationToken cancellationToken = default);
    Task<ServiceResult<UserResponseDto>> ActivateAsync(int id, CancellationToken cancellationToken = default);
    Task<ServiceResult> DeleteAsync(int id, CancellationToken cancellationToken = default);

}