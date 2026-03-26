using Lakbay.Application.DTOs.Auth;

namespace Lakbay.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    Task<LoginResponse?> RegisterAsync(RegisterRequest request);
    Task<LoginResponse?> RefreshTokenAsync(string refreshToken, string? ipAddress = null);
    Task<bool> RevokeTokenAsync(string refreshToken, string? ipAddress = null);
    Task<bool> LogoutAsync(Guid userId);
}
