namespace Piloopinas.Application.DTOs.Dashboard;

public record DashboardStatsDto(
    int TotalRiders,
    int TotalEvents,
    int ActiveEvents,
    int TotalRegistrations,
    decimal TotalDistanceCompleted,
    List<UpcomingEventDto> UpcomingEvents,
    List<LeaderboardEntryDto> TopRiders
);

public record UpcomingEventDto(
    Guid Id,
    string Name,
    string EventType,
    DateTime StartDateTime,
    string StartLocation,
    int CurrentParticipants,
    int MaxParticipants
);

public record LeaderboardEntryDto(
    int Rank,
    Guid UserId,
    string RiderName,
    string? ProfilePhotoUrl,
    int TotalPoints,
    decimal TotalDistanceKm,
    int TotalEventsCompleted
);

public record RiderDashboardDto(
    int TotalPoints,
    decimal TotalDistanceKm,
    int TotalEventsCompleted,
    int CurrentRank,
    List<UpcomingEventDto> MyUpcomingEvents,
    List<BadgeSummaryDto> RecentBadges,
    List<RecentActivityDto> RecentActivities
);

public record BadgeSummaryDto(
    int Id,
    string Name,
    string? IconUrl,
    DateTime EarnedAt
);

public record RecentActivityDto(
    string Type,
    string Description,
    DateTime OccurredAt
);
