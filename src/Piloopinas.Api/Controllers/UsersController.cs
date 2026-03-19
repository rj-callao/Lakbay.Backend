using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piloopinas.Application.DTOs.Users;
using Piloopinas.Application.Interfaces;

namespace Piloopinas.Api.Controllers;

[Route("api/v1/users")]
[Authorize]
public class UsersController : AppBaseController
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        [FromQuery] string? sortField = null,
        [FromQuery] string? sortOrder = null,
        [FromQuery] string? search = null,
        [FromQuery] string? role = null,
        [FromQuery] bool? isActive = null)
    {
        var result = await _userService.GetUsersAsync(
            page, pageSize, sortField, sortOrder, search, role, isActive);
        
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var result = await _userService.GetUserByIdAsync(id);
        
        if (result == null)
            return NotFound(new { error = new { message = "User not found" } });
        
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        var result = await _userService.CreateUserAsync(request, GetCurrentUserId().ToString());
        return CreatedAtAction(nameof(GetUser), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest request)
    {
        var result = await _userService.UpdateUserAsync(id, request, GetCurrentUserId().ToString());
        
        if (result == null)
            return NotFound(new { error = new { message = "User not found" } });
        
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var result = await _userService.DeleteUserAsync(id);
        
        if (!result)
            return NotFound(new { error = new { message = "User not found" } });
        
        return NoContent();
    }
}
