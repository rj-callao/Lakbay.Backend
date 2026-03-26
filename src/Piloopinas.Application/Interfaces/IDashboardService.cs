using Lakbay.Application.DTOs.Dashboard;

namespace Lakbay.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardStatsDto> GetDashboardStatsAsync();
    Task<RiderDashboardDto> GetRiderDashboardAsync(Guid userId);
    Task<List<LeaderboardEntryDto>> GetLeaderboardAsync(
        string? period = null, // all-time, monthly, weekly
        string? region = null,
        int limit = 10);
}
