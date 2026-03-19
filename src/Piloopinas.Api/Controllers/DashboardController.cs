using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piloopinas.Application.Interfaces;

namespace Piloopinas.Api.Controllers;

[Route("api/v1/dashboard")]
[Authorize]
public class DashboardController : AppBaseController
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Organizer")]
    public async Task<IActionResult> GetDashboardStats()
    {
        var result = await _dashboardService.GetDashboardStatsAsync();
        return Ok(result);
    }

    [HttpGet("rider")]
    public async Task<IActionResult> GetRiderDashboard()
    {
        var userId = GetCurrentUserId();
        var result = await _dashboardService.GetRiderDashboardAsync(userId);
        return Ok(result);
    }

    [HttpGet("leaderboard")]
    [AllowAnonymous]
    public async Task<IActionResult> GetLeaderboard(
        [FromQuery] string? period = null,
        [FromQuery] string? region = null,
        [FromQuery] int limit = 10)
    {
        var result = await _dashboardService.GetLeaderboardAsync(period, region, limit);
        return Ok(result);
    }
}
