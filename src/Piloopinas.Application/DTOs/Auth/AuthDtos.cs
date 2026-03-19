using System.ComponentModel.DataAnnotations;

namespace Piloopinas.Application.DTOs.Auth;

public record LoginRequest(
    [Required] [EmailAddress] string Email,
    [Required] string Password
);

public record RegisterRequest(
    [Required] [EmailAddress] string Email,
    [Required] [MinLength(6)] string Password,
    [Required] [StringLength(100)] string FirstName,
    [Required] [StringLength(100)] string LastName,
    [Phone] string? PhoneNumber
);

public record LoginResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    UserDto User
);

public record RefreshTokenRequest(
    [Required] string RefreshToken
);

public record UserDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string? ProfilePhotoUrl,
    string Role,
    decimal TotalDistanceKm,
    int TotalEventsCompleted,
    int TotalPoints
);
