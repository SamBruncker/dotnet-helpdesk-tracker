using HelpdeskTracker.Api.Dtos;
using HelpdeskTracker.Api.Services.Results;
using Microsoft.EntityFrameworkCore;
using HelpdeskTracker.Api.Data;
using HelpdeskTracker.Api.Domain;

namespace HelpdeskTracker.Api.Services;

public class UserService : IUserService
{
    private readonly HelpdeskDbContext _dbContext;
    public UserService(HelpdeskDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<UserResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await _dbContext.Users
            .OrderBy(u => u.DisplayName)
            .Select(u => new UserResponseDto
            {
                Id = u.Id,
                DisplayName = u.DisplayName,
                Email = u.Email,
                Role = u.Role,
                IsActive = u.IsActive
            })
            .ToListAsync(cancellationToken);

        return users;
    }

    public async Task<ServiceResult<UserResponseDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users
            .Select(u => new UserResponseDto
            {
                Id = u.Id,
                DisplayName = u.DisplayName,
                Email = u.Email,
                Role = u.Role,
                IsActive = u.IsActive
            })
            .Where(u => u.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            return ServiceResult<UserResponseDto>.Fail(ErrorKind.NotFound, "User not found");
        }

        return ServiceResult<UserResponseDto>.Ok(user);
    }

    public Task<ServiceResult<UserResponseDto>> CreateAsync(CreateUserRequestDto request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResult<UserResponseDto>> UpdateAsync(int id, UpdateUserRequestDto request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResult<UserResponseDto>> DeactivateAsync(int id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResult<UserResponseDto>> ActivateAsync(int id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}