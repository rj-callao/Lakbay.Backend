using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Lakbay.Application.DTOs.Users;
using Lakbay.Application.Interfaces;

namespace Lakbay.Api.Controllers;

[Route("api/v1/profile")]
[Authorize]
public class ProfileController : AppBaseController
{
    private readonly IUserService _userService;
    private readonly IMotorcycleService _motorcycleService;

    public ProfileController(IUserService userService, IMotorcycleService motorcycleService)
    {
        _userService = userService;
        _motorcycleService = motorcycleService;
    }

    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var userId = GetCurrentUserId();
        var result = await _userService.GetUserProfileAsync(userId);
        
        if (result == null)
            return NotFound(new { error = new { message = "Profile not found" } });
        
        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var userId = GetCurrentUserId();
        var result = await _userService.UpdateProfileAsync(userId, request);
        
        if (!result)
            return NotFound(new { error = new { message = "Profile not found" } });
        
        return Ok(new { message = "Profile updated successfully" });
    }

    [HttpGet("motorcycles")]
    public async Task<IActionResult> GetMyMotorcycles()
    {
        var userId = GetCurrentUserId();
        var result = await _motorcycleService.GetUserMotorcyclesAsync(userId);
        return Ok(result);
    }

    [HttpPost("motorcycles")]
    public async Task<IActionResult> AddMotorcycle([FromBody] CreateMotorcycleRequest request)
    {
        var userId = GetCurrentUserId();
        var result = await _motorcycleService.CreateMotorcycleAsync(userId, request);
        return CreatedAtAction(nameof(GetMotorcycle), new { id = result.Id }, result);
    }

    [HttpGet("motorcycles/{id:guid}")]
    public async Task<IActionResult> GetMotorcycle(Guid id)
    {
        var userId = GetCurrentUserId();
        var result = await _motorcycleService.GetMotorcycleByIdAsync(id, userId);
        
        if (result == null)
            return NotFound(new { error = new { message = "Motorcycle not found" } });
        
        return Ok(result);
    }

    [HttpPut("motorcycles/{id:guid}")]
    public async Task<IActionResult> UpdateMotorcycle(Guid id, [FromBody] UpdateMotorcycleRequest request)
    {
        var userId = GetCurrentUserId();
        var result = await _motorcycleService.UpdateMotorcycleAsync(id, userId, request);
        
        if (result == null)
            return NotFound(new { error = new { message = "Motorcycle not found" } });
        
        return Ok(result);
    }

    [HttpDelete("motorcycles/{id:guid}")]
    public async Task<IActionResult> DeleteMotorcycle(Guid id)
    {
        var userId = GetCurrentUserId();
        var result = await _motorcycleService.DeleteMotorcycleAsync(id, userId);
        
        if (!result)
            return NotFound(new { error = new { message = "Motorcycle not found" } });
        
        return NoContent();
    }

    [HttpPatch("motorcycles/{id:guid}/primary")]
    public async Task<IActionResult> SetPrimaryMotorcycle(Guid id)
    {
        var userId = GetCurrentUserId();
        var result = await _motorcycleService.SetPrimaryMotorcycleAsync(id, userId);
        
        if (!result)
            return NotFound(new { error = new { message = "Motorcycle not found" } });
        
        return Ok(new { message = "Primary motorcycle updated successfully" });
    }
}
