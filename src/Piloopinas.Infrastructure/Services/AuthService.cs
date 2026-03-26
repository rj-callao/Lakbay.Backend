using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Lakbay.Application.DTOs.Auth;
using Lakbay.Application.Interfaces;
using Lakbay.Domain;
using Lakbay.Domain.Entities;
using Lakbay.Infrastructure.Data;

namespace Lakbay.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users
            .AsNoTracking()
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == request.Email && u.IsActive);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return null;

        return await GenerateTokensAsync(user);
    }

    public async Task<LoginResponse?> RegisterAsync(RegisterRequest request)
    {
        // Check if email already exists
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            return null;

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            RoleId = RoleConstants.Rider,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Reload with role
        user = await _context.Users
            .AsNoTracking()
            .Include(u => u.Role)
            .FirstAsync(u => u.Id == user.Id);

        return await GenerateTokensAsync(user);
    }

    public async Task<LoginResponse?> RefreshTokenAsync(string refreshToken, string? ipAddress = null)
    {
        var tokenHash = HashToken(refreshToken);
        var storedToken = await _context.RefreshTokens
            .Include(rt => rt.User)
                .ThenInclude(u => u.Role)
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash);

        if (storedToken == null || !storedToken.IsActive || !storedToken.User.IsActive)
            return null;

        // Revoke old token and create new one
        storedToken.RevokedAt = DateTime.UtcNow;
        storedToken.RevokedByIp = ipAddress;

        var response = await GenerateTokensAsync(storedToken.User, ipAddress);
        
        storedToken.ReplacedByTokenHash = HashToken(response.RefreshToken);
        await _context.SaveChangesAsync();

        return response;
    }

    public async Task<bool> RevokeTokenAsync(string refreshToken, string? ipAddress = null)
    {
        var tokenHash = HashToken(refreshToken);
        var storedToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash);

        if (storedToken == null || !storedToken.IsActive)
            return false;

        storedToken.RevokedAt = DateTime.UtcNow;
        storedToken.RevokedByIp = ipAddress;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> LogoutAsync(Guid userId)
    {
        var tokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && rt.RevokedAt == null)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.RevokedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    private async Task<LoginResponse> GenerateTokensAsync(User user, string? ipAddress = null)
    {
        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken();
        var expiresAt = DateTime.UtcNow.AddMinutes(
            double.Parse(_configuration["Jwt:ExpiresInMinutes"] ?? "1440"));

        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = HashToken(refreshToken),
            ExpiresAt = DateTime.UtcNow.AddDays(
                double.Parse(_configuration["Jwt:RefreshTokenExpiresInDays"] ?? "7")),
            CreatedAt = DateTime.UtcNow,
            CreatedByIp = ipAddress
        };

        _context.RefreshTokens.Add(refreshTokenEntity);
        await _context.SaveChangesAsync();

        return new LoginResponse(
            accessToken,
            refreshToken,
            expiresAt,
            new UserDto(
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName,
                user.ProfilePhotoUrl,
                user.Role.Name,
                user.TotalDistanceKm,
                user.TotalEventsCompleted,
                user.TotalPoints
            )
        );
    }

    private string GenerateAccessToken(User user)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.GivenName, user.FirstName),
            new Claim(ClaimTypes.Surname, user.LastName),
            new Claim(ClaimTypes.Role, user.Role.Name)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                double.Parse(_configuration["Jwt:ExpiresInMinutes"] ?? "1440")),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    private static string HashToken(string token)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(bytes);
    }
}
