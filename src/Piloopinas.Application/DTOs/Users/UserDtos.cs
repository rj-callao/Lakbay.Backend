using System.ComponentModel.DataAnnotations;

namespace Lakbay.Application.DTOs.Users;

public record UserProfileDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string? ProfilePhotoUrl,
    string? PhoneNumber,
    string? Address,
    string? City,
    string? Province,
    decimal TotalDistanceKm,
    int TotalEventsCompleted,
    int TotalPoints,
    string? FacebookUrl,
    string? InstagramUrl,
    string Role,
    bool IsActive,
    DateTime CreatedAt,
    List<MotorcycleDto> Motorcycles,
    List<BadgeDto> Badges
);

public record MotorcycleDto(
    Guid Id,
    string Brand,
    string Model,
    int? Year,
    string? PlateNumber,
    string? Color,
    int? EngineDisplacementCc,
    string? PhotoUrl,
    bool IsPrimary
);

public record BadgeDto(
    int Id,
    string Name,
    string? Description,
    string? IconUrl,
    string Category,
    int PointsValue,
    DateTime? EarnedAt
);

public record UpdateProfileRequest(
    [Required] [StringLength(100)] string FirstName,
    [Required] [StringLength(100)] string LastName,
    string? ProfilePhotoUrl,
    [Phone] string? PhoneNumber,
    string? Address,
    string? City,
    string? Province,
    string? FacebookUrl,
    string? InstagramUrl
);

public record CreateMotorcycleRequest(
    [Required] [StringLength(100)] string Brand,
    [Required] [StringLength(100)] string Model,
    int? Year,
    string? PlateNumber,
    string? Color,
    int? EngineDisplacementCc,
    string? PhotoUrl,
    bool IsPrimary = false
);

public record UpdateMotorcycleRequest(
    [Required] [StringLength(100)] string Brand,
    [Required] [StringLength(100)] string Model,
    int? Year,
    string? PlateNumber,
    string? Color,
    int? EngineDisplacementCc,
    string? PhotoUrl,
    bool IsPrimary
);

// Admin user management
public record AdminUserDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    string Role,
    bool IsActive,
    decimal TotalDistanceKm,
    int TotalEventsCompleted,
    int TotalPoints,
    DateTime CreatedAt
);

public record CreateUserRequest(
    [Required] [EmailAddress] string Email,
    [Required] [MinLength(6)] string Password,
    [Required] [StringLength(100)] string FirstName,
    [Required] [StringLength(100)] string LastName,
    [Phone] string? PhoneNumber,
    int RoleId = 3
);

public record UpdateUserRequest(
    [Required] [StringLength(100)] string FirstName,
    [Required] [StringLength(100)] string LastName,
    [Phone] string? PhoneNumber,
    int RoleId,
    bool IsActive
);

// Public rider profile (visible to other users)
public record RiderProfileDto(
    Guid Id,
    string FirstName,
    string LastName,
    string? ProfilePhotoUrl,
    string? Province,
    decimal TotalDistanceKm,
    int TotalEventsCompleted,
    int TotalPoints,
    string? FacebookUrl,
    string? InstagramUrl,
    DateTime CreatedAt,
    int FollowersCount,
    int FollowingCount,
    bool IsFollowedByCurrentUser,
    List<MotorcycleDto> Motorcycles,
    List<BadgeDto> Badges,
    List<RiderEventSummaryDto> RecentEvents
);

public record RiderEventSummaryDto(
    Guid EventId,
    string EventName,
    string EventType,
    string Difficulty,
    decimal DistanceKm,
    int? PointsEarned,
    bool IsCompleted,
    DateTime RegisteredAt
);

public record FollowResultDto(
    bool IsFollowing,
    int FollowersCount
);
