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

    public async Task<ServiceResult<UserResponseDto>> CreateAsync(CreateUserRequestDto request, CancellationToken cancellationToken = default)
    {
        // Only need to trim. SQL Server compares cases insensitively.
        string displayName = request.DisplayName.Trim();
        string email = request.Email.Trim();

        bool emailExists = await _dbContext.Users.AnyAsync(u => u.Email == email, cancellationToken);

        if (emailExists)
        {
            return ServiceResult<UserResponseDto>.Fail(ErrorKind.Conflict, "Email already exists");
        }

        User user = new User
        {
            DisplayName = displayName,
            Email = email,
            Role = request.Role,
            IsActive = true
        };

        try
        {
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        //Handle a race case where two requests are made with the same email.
        catch (DbUpdateException)
        {
            //This is a bit of a catch all at the moment. In future, tighten this error to reduce likelihood
            //of an unrelated error returning an email related message.
            return ServiceResult<UserResponseDto>.Fail(ErrorKind.Conflict, "Email already exists");
        }

        return ServiceResult<UserResponseDto>.Ok(MapToDto(user));

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

    private static UserResponseDto MapToDto(User user)
    {
        return new UserResponseDto
        {
            Id = user.Id,
            DisplayName = user.DisplayName,
            Email = user.Email,
            Role = user.Role,
            IsActive = user.IsActive
        };
    }
}