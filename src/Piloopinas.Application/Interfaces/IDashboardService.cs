using Piloopinas.Application.DTOs.Dashboard;

namespace Piloopinas.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardStatsDto> GetDashboardStatsAsync();
    Task<RiderDashboardDto> GetRiderDashboardAsync(Guid userId);
    Task<List<LeaderboardEntryDto>> GetLeaderboardAsync(
        string? period = null, // all-time, monthly, weekly
        string? region = null,
        int limit = 10);
}
