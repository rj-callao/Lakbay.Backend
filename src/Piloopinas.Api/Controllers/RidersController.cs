using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Lakbay.Application.Interfaces;

namespace Lakbay.Api.Controllers;

[Route("api/v1/riders")]
[Authorize]
public class RidersController : AppBaseController
{
    private readonly IUserService _userService;

    public RidersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetRiderProfile(Guid id)
    {
        var currentUserId = GetCurrentUserId();
        var result = await _userService.GetRiderProfileAsync(id, currentUserId);
        if (result == null)
            return NotFound(new { error = new { message = "Rider not found" } });
        return Ok(result);
    }

    [HttpPost("{id:guid}/follow")]
    public async Task<IActionResult> ToggleFollow(Guid id)
    {
        var currentUserId = GetCurrentUserId();
        var result = await _userService.ToggleFollowAsync(currentUserId, id);
        if (result == null)
            return BadRequest(new { error = new { message = "Cannot follow this user." } });
        return Ok(result);
    }

    [HttpGet("{id:guid}/followers")]
    public async Task<IActionResult> GetFollowers(Guid id)
    {
        var currentUserId = GetCurrentUserId();
        var result = await _userService.GetFollowersAsync(id, currentUserId);
        return Ok(result);
    }

    [HttpGet("{id:guid}/following")]
    public async Task<IActionResult> GetFollowing(Guid id)
    {
        var currentUserId = GetCurrentUserId();
        var result = await _userService.GetFollowingAsync(id, currentUserId);
        return Ok(result);
    }
}
