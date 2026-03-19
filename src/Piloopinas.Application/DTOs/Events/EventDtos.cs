using System.ComponentModel.DataAnnotations;

namespace Piloopinas.Application.DTOs.Events;

public record EventDto(
    Guid Id,
    string Name,
    string? Description,
    string? ImageUrl,
    string EventType,
    string Difficulty,
    decimal DistanceKm,
    string StartLocation,
    string? EndLocation,
    string? Region,
    string? Province,
    DateTime StartDateTime,
    DateTime? EndDateTime,
    DateTime RegistrationDeadline,
    int MaxParticipants,
    int CurrentParticipants,
    decimal RegistrationFee,
    int PointsReward,
    string Status,
    DateTime CreatedAt
);

public record EventDetailDto(
    Guid Id,
    string Name,
    string? Description,
    string? ImageUrl,
    string EventType,
    string Difficulty,
    decimal DistanceKm,
    string? Route,
    string StartLocation,
    string? EndLocation,
    string? Region,
    string? Province,
    DateTime StartDateTime,
    DateTime? EndDateTime,
    DateTime RegistrationDeadline,
    int MaxParticipants,
    int CurrentParticipants,
    decimal RegistrationFee,
    int PointsReward,
    string Status,
    DateTime CreatedAt,
    List<EventParticipantDto> Participants
);

public record EventParticipantDto(
    Guid UserId,
    string RiderName,
    string? ProfilePhotoUrl,
    string? MotorcycleInfo,
    string RegistrationStatus,
    DateTime RegisteredAt,
    bool IsCompleted,
    int? FinishPosition
);

public record CreateEventRequest(
    [Required] [StringLength(200)] string Name,
    string? Description,
    string? ImageUrl,
    [Required] string EventType,
    [Required] string Difficulty,
    [Required] [Range(1, 10000)] decimal DistanceKm,
    string? Route,
    [Required] [StringLength(500)] string StartLocation,
    string? EndLocation,
    string? Region,
    string? Province,
    [Required] DateTime StartDateTime,
    DateTime? EndDateTime,
    [Required] DateTime RegistrationDeadline,
    [Required] [Range(1, 10000)] int MaxParticipants,
    [Required] [Range(0, 100000)] decimal RegistrationFee,
    [Range(0, 10000)] int PointsReward = 100
);

public record UpdateEventRequest(
    [Required] [StringLength(200)] string Name,
    string? Description,
    string? ImageUrl,
    [Required] string EventType,
    [Required] string Difficulty,
    [Required] [Range(1, 10000)] decimal DistanceKm,
    string? Route,
    [Required] [StringLength(500)] string StartLocation,
    string? EndLocation,
    string? Region,
    string? Province,
    [Required] DateTime StartDateTime,
    DateTime? EndDateTime,
    [Required] DateTime RegistrationDeadline,
    [Required] [Range(1, 10000)] int MaxParticipants,
    [Required] [Range(0, 100000)] decimal RegistrationFee,
    [Range(0, 10000)] int PointsReward,
    int StatusId
);

public record RegisterForEventRequest(
    Guid? MotorcycleId
);

public record EventRegistrationDto(
    Guid Id,
    Guid EventId,
    string EventName,
    Guid UserId,
    string RiderName,
    string RegistrationStatus,
    string? PaymentStatus,
    decimal AmountPaid,
    bool IsCompleted,
    DateTime? CompletedAt,
    int? PointsEarned,
    int? FinishPosition,
    DateTime RegisteredAt
);
