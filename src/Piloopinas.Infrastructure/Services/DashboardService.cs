using Microsoft.EntityFrameworkCore;
using Piloopinas.Application.DTOs.Dashboard;
using Piloopinas.Application.Interfaces;
using Piloopinas.Domain;
using Piloopinas.Infrastructure.Data;

namespace Piloopinas.Infrastructure.Services;

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _context;

    public DashboardService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardStatsDto> GetDashboardStatsAsync()
    {
        var totalRiders = await _context.Users
            .CountAsync(u => u.RoleId == RoleConstants.Rider && u.IsActive);

        var totalEvents = await _context.Events.CountAsync();

        var activeEvents = await _context.Events
            .CountAsync(e => e.StatusId == EventStatusConstants.RegistrationOpen ||
                            e.StatusId == EventStatusConstants.InProgress);

        var totalRegistrations = await _context.EventRegistrations
            .CountAsync(r => r.RegistrationStatus != "Cancelled");

        var totalDistanceCompleted = await _context.EventRegistrations
            .Where(r => r.IsCompleted && r.ActualDistanceKm.HasValue)
            .SumAsync(r => r.ActualDistanceKm ?? 0);

        var upcomingEvents = await _context.Events
            .AsNoTracking()
            .Include(e => e.Registrations)
            .Where(e => e.StartDateTime > DateTime.UtcNow &&
                       (e.StatusId == EventStatusConstants.RegistrationOpen ||
                        e.StatusId == EventStatusConstants.Published))
            .OrderBy(e => e.StartDateTime)
            .Take(5)
            .Select(e => new UpcomingEventDto(
                e.Id,
                e.Name,
                e.EventType,
                e.StartDateTime,
                e.StartLocation,
                e.Registrations.Count(r => r.RegistrationStatus != "Cancelled"),
                e.MaxParticipants
            ))
            .ToListAsync();

        var topRiders = await GetLeaderboardAsync(limit: 5);

        return new DashboardStatsDto(
            totalRiders,
            totalEvents,
            activeEvents,
            totalRegistrations,
            totalDistanceCompleted,
            upcomingEvents,
            topRiders
        );
    }

    public async Task<RiderDashboardDto> GetRiderDashboardAsync(Guid userId)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return new RiderDashboardDto(0, 0, 0, 0, [], [], []);

        // Calculate rank
        var rank = await _context.Users
            .CountAsync(u => u.TotalPoints > user.TotalPoints && u.IsActive) + 1;

        // Get upcoming events the user is registered for
        var myUpcomingEvents = await _context.EventRegistrations
            .AsNoTracking()
            .Include(r => r.Event)
                .ThenInclude(e => e.Registrations)
            .Where(r => r.UserId == userId &&
                       r.RegistrationStatus != "Cancelled" &&
                       r.Event.StartDateTime > DateTime.UtcNow)
            .OrderBy(r => r.Event.StartDateTime)
            .Take(5)
            .Select(r => new UpcomingEventDto(
                r.Event.Id,
                r.Event.Name,
                r.Event.EventType,
                r.Event.StartDateTime,
                r.Event.StartLocation,
                r.Event.Registrations.Count(reg => reg.RegistrationStatus != "Cancelled"),
                r.Event.MaxParticipants
            ))
            .ToListAsync();

        // Get recent badges
        var recentBadges = await _context.UserBadges
            .AsNoTracking()
            .Include(ub => ub.Badge)
            .Where(ub => ub.UserId == userId)
            .OrderByDescending(ub => ub.EarnedAt)
            .Take(5)
            .Select(ub => new BadgeSummaryDto(
                ub.Badge.Id,
                ub.Badge.Name,
                ub.Badge.IconUrl,
                ub.EarnedAt
            ))
            .ToListAsync();

        // Get recent activities (completed events)
        var recentActivities = await _context.EventRegistrations
            .AsNoTracking()
            .Include(r => r.Event)
            .Where(r => r.UserId == userId && r.IsCompleted)
            .OrderByDescending(r => r.CompletedAt)
            .Take(5)
            .Select(r => new RecentActivityDto(
                "EventCompleted",
                $"Completed {r.Event.Name}",
                r.CompletedAt ?? r.UpdatedAt ?? r.CreatedAt
            ))
            .ToListAsync();

        return new RiderDashboardDto(
            user.TotalPoints,
            user.TotalDistanceKm,
            user.TotalEventsCompleted,
            rank,
            myUpcomingEvents,
            recentBadges,
            recentActivities
        );
    }

    public async Task<List<LeaderboardEntryDto>> GetLeaderboardAsync(
        string? period = null,
        string? region = null,
        int limit = 10)
    {
        var query = _context.Users
            .AsNoTracking()
            .Where(u => u.RoleId == RoleConstants.Rider && u.IsActive);

        // Filter by region if specified
        if (!string.IsNullOrEmpty(region))
        {
            query = query.Where(u => u.Province == region);
        }

        // For period-based filtering, we would need to track points/distance by period
        // For now, we use total stats
        var users = await query
            .OrderByDescending(u => u.TotalPoints)
            .ThenByDescending(u => u.TotalDistanceKm)
            .Take(limit)
            .Select(u => new
            {
                u.Id,
                u.FirstName,
                u.LastName,
                u.ProfilePhotoUrl,
                u.TotalPoints,
                u.TotalDistanceKm,
                u.TotalEventsCompleted
            })
            .ToListAsync();

        return users.Select((u, index) => new LeaderboardEntryDto(
            index + 1,
            u.Id,
            $"{u.FirstName} {u.LastName}",
            u.ProfilePhotoUrl,
            u.TotalPoints,
            u.TotalDistanceKm,
            u.TotalEventsCompleted
        )).ToList();
    }
}
