using HelpdeskTracker.Api.Services;
using HelpdeskTracker.Api.Dtos;
using HelpdeskTracker.Api.Services.Results;
using Microsoft.AspNetCore.Mvc;

namespace HelpdeskTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _userService.GetAllAsync(cancellationToken));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserResponseDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _userService.GetByIdAsync(id, cancellationToken);

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
    public async Task<ActionResult<UserResponseDto>> CreateUser([FromBody] CreateUserRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _userService.CreateAsync(request, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.Error switch
            {
                ErrorKind.Conflict => Conflict(result.ErrorMessage),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result.ErrorMessage)
            };
        }
        return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
    }


    [HttpPost("{id:int}/activate")]
    public async Task<ActionResult<UserResponseDto>> ActivateUser(int id, CancellationToken cancellationToken)
    {
        var result = await _userService.ActivateAsync(id, cancellationToken);

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

    [HttpPost("{id:int}/deactivate")]
    public async Task<ActionResult<UserResponseDto>> DeactivateUser(int id, CancellationToken cancellationToken)
    {
        var result = await _userService.DeactivateAsync(id, cancellationToken);

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


    [HttpPut("{id:int}")]
    public async Task<ActionResult<UserResponseDto>> UpdateUser(int id, [FromBody] UpdateUserRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _userService.UpdateAsync(id, request, cancellationToken);

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

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _userService.DeleteAsync(id, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.Error switch
            {
                ErrorKind.NotFound => NotFound(result.ErrorMessage),
                ErrorKind.Conflict => Conflict(result.ErrorMessage),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result.ErrorMessage)
            };
        }
        return NoContent();
    }


}

