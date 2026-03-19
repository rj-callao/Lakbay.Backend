using Microsoft.EntityFrameworkCore;
using Piloopinas.Application.DTOs.Common;
using Piloopinas.Application.DTOs.Users;
using Piloopinas.Application.Interfaces;
using Piloopinas.Domain;
using Piloopinas.Domain.Entities;
using Piloopinas.Infrastructure.Data;

namespace Piloopinas.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<AdminUserDto>> GetUsersAsync(
        int page, int pageSize,
        string? sortField = null, string? sortOrder = null,
        string? search = null, string? role = null,
        bool? isActive = null)
    {
        var query = _context.Users
            .AsNoTracking()
            .Include(u => u.Role)
            .AsQueryable();

        // Apply filters
        if (!string.IsNullOrEmpty(search))
            query = query.Where(u => 
                u.FirstName.Contains(search) || 
                u.LastName.Contains(search) || 
                u.Email.Contains(search));

        if (!string.IsNullOrEmpty(role))
        {
            var roles = role.Split(',', StringSplitOptions.RemoveEmptyEntries);
            query = query.Where(u => roles.Contains(u.Role.Name));
        }

        if (isActive.HasValue)
            query = query.Where(u => u.IsActive == isActive.Value);

        var totalCount = await query.CountAsync();

        // Apply sorting
        query = (sortField?.ToLower(), sortOrder?.ToLower()) switch
        {
            ("firstname", "asc") => query.OrderBy(u => u.FirstName),
            ("firstname", _) => query.OrderByDescending(u => u.FirstName),
            ("lastname", "asc") => query.OrderBy(u => u.LastName),
            ("lastname", _) => query.OrderByDescending(u => u.LastName),
            ("email", "asc") => query.OrderBy(u => u.Email),
            ("email", _) => query.OrderByDescending(u => u.Email),
            ("totalpoints", "asc") => query.OrderBy(u => u.TotalPoints),
            ("totalpoints", _) => query.OrderByDescending(u => u.TotalPoints),
            _ => query.OrderByDescending(u => u.CreatedAt)
        };

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new AdminUserDto(
                u.Id,
                u.Email,
                u.FirstName,
                u.LastName,
                u.PhoneNumber,
                u.Role.Name,
                u.IsActive,
                u.TotalDistanceKm,
                u.TotalEventsCompleted,
                u.TotalPoints,
                u.CreatedAt
            ))
            .ToListAsync();

        return new PagedResult<AdminUserDto>(items, totalCount, page, pageSize);
    }

    public async Task<UserProfileDto?> GetUserByIdAsync(Guid id)
    {
        return await GetUserProfileAsync(id);
    }

    public async Task<UserProfileDto?> GetUserProfileAsync(Guid userId)
    {
        var user = await _context.Users
            .AsNoTracking()
            .Include(u => u.Role)
            .Include(u => u.Motorcycles.Where(m => m.IsActive))
            .Include(u => u.UserBadges)
                .ThenInclude(ub => ub.Badge)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) return null;

        return new UserProfileDto(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            user.ProfilePhotoUrl,
            user.PhoneNumber,
            user.Address,
            user.City,
            user.Province,
            user.TotalDistanceKm,
            user.TotalEventsCompleted,
            user.TotalPoints,
            user.FacebookUrl,
            user.InstagramUrl,
            user.Role.Name,
            user.IsActive,
            user.CreatedAt,
            user.Motorcycles.Select(m => new MotorcycleDto(
                m.Id,
                m.Brand,
                m.Model,
                m.Year,
                m.PlateNumber,
                m.Color,
                m.EngineDisplacementCc,
                m.PhotoUrl,
                m.IsPrimary
            )).ToList(),
            user.UserBadges.Select(ub => new BadgeDto(
                ub.Badge.Id,
                ub.Badge.Name,
                ub.Badge.Description,
                ub.Badge.IconUrl,
                ub.Badge.Category,
                ub.Badge.PointsValue,
                ub.EarnedAt
            )).ToList()
        );
    }

    public async Task<AdminUserDto> CreateUserAsync(CreateUserRequest request, string createdBy)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            RoleId = request.RoleId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var role = await _context.Roles.FindAsync(request.RoleId);

        return new AdminUserDto(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            user.PhoneNumber,
            role?.Name ?? "Unknown",
            user.IsActive,
            user.TotalDistanceKm,
            user.TotalEventsCompleted,
            user.TotalPoints,
            user.CreatedAt
        );
    }

    public async Task<AdminUserDto?> UpdateUserAsync(Guid id, UpdateUserRequest request, string updatedBy)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null) return null;

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.PhoneNumber = request.PhoneNumber;
        user.RoleId = request.RoleId;
        user.IsActive = request.IsActive;
        user.UpdatedAt = DateTime.UtcNow;
        user.UpdatedBy = updatedBy;

        await _context.SaveChangesAsync();

        var role = await _context.Roles.FindAsync(request.RoleId);

        return new AdminUserDto(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            user.PhoneNumber,
            role?.Name ?? "Unknown",
            user.IsActive,
            user.TotalDistanceKm,
            user.TotalEventsCompleted,
            user.TotalPoints,
            user.CreatedAt
        );
    }

    public async Task<bool> DeleteUserAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return false;

        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateProfileAsync(Guid userId, UpdateProfileRequest request)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.ProfilePhotoUrl = request.ProfilePhotoUrl;
        user.PhoneNumber = request.PhoneNumber;
        user.Address = request.Address;
        user.City = request.City;
        user.Province = request.Province;
        user.FacebookUrl = request.FacebookUrl;
        user.InstagramUrl = request.InstagramUrl;
        user.UpdatedAt = DateTime.UtcNow;
        user.UpdatedBy = userId.ToString();

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<RiderProfileDto?> GetRiderProfileAsync(Guid riderId, Guid? currentUserId)
    {
        var user = await _context.Users.AsNoTracking()
            .Include(u => u.Motorcycles.Where(m => m.IsActive))
            .Include(u => u.UserBadges).ThenInclude(ub => ub.Badge)
            .Include(u => u.Followers)
            .Include(u => u.Following)
            .FirstOrDefaultAsync(u => u.Id == riderId && u.RoleId == RoleConstants.Rider);

        if (user == null) return null;

        var isFollowed = currentUserId.HasValue &&
            await _context.UserFollows.AnyAsync(f => f.FollowerId == currentUserId.Value && f.FollowingId == riderId);

        var recentEvents = await _context.EventRegistrations.AsNoTracking()
            .Include(r => r.Event)
            .Where(r => r.UserId == riderId && r.RegistrationStatus != "Cancelled")
            .OrderByDescending(r => r.RegisteredAt)
            .Take(10)
            .Select(r => new RiderEventSummaryDto(
                r.EventId, r.Event.Name, r.Event.EventType, r.Event.Difficulty,
                r.Event.DistanceKm, r.PointsEarned, r.IsCompleted, r.RegisteredAt
            )).ToListAsync();

        return new RiderProfileDto(
            user.Id, user.FirstName, user.LastName, user.ProfilePhotoUrl, user.Province,
            user.TotalDistanceKm, user.TotalEventsCompleted, user.TotalPoints,
            user.FacebookUrl, user.InstagramUrl, user.CreatedAt,
            user.Followers.Count, user.Following.Count, isFollowed,
            user.Motorcycles.Select(m => new MotorcycleDto(m.Id, m.Brand, m.Model, m.Year, m.PlateNumber, m.Color, m.EngineDisplacementCc, m.PhotoUrl, m.IsPrimary)).ToList(),
            user.UserBadges.Select(ub => new BadgeDto(ub.Badge.Id, ub.Badge.Name, ub.Badge.Description, ub.Badge.IconUrl, ub.Badge.Category, ub.Badge.PointsValue, ub.EarnedAt)).ToList(),
            recentEvents
        );
    }

    public async Task<FollowResultDto?> ToggleFollowAsync(Guid followerId, Guid followingId)
    {
        if (followerId == followingId) return null;

        var followerUser = await _context.Users.FindAsync(followerId);
        if (followerUser == null) return null;

        var targetUser = await _context.Users.FindAsync(followingId);
        if (targetUser == null || targetUser.RoleId != RoleConstants.Rider) return null;

        var existingFollow = await _context.UserFollows
            .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowingId == followingId);

        if (existingFollow != null)
        {
            _context.UserFollows.Remove(existingFollow);
            await _context.SaveChangesAsync();
            var count = await _context.UserFollows.CountAsync(f => f.FollowingId == followingId);
            return new FollowResultDto(false, count);
        }
        else
        {
            _context.UserFollows.Add(new UserFollow
            {
                Id = Guid.NewGuid(),
                FollowerId = followerId,
                FollowingId = followingId,
                FollowedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
            var count = await _context.UserFollows.CountAsync(f => f.FollowingId == followingId);
            return new FollowResultDto(true, count);
        }
    }

    public async Task<List<RiderProfileDto>> GetFollowersAsync(Guid userId, Guid? currentUserId)
    {
        var followers = await _context.UserFollows.AsNoTracking()
            .Include(f => f.Follower)
            .Where(f => f.FollowingId == userId)
            .Select(f => f.Follower)
            .ToListAsync();

        return followers.Select(u => new RiderProfileDto(
            u.Id, u.FirstName, u.LastName, u.ProfilePhotoUrl, u.Province,
            u.TotalDistanceKm, u.TotalEventsCompleted, u.TotalPoints,
            null, null, u.CreatedAt, 0, 0, false, new(), new(), new()
        )).ToList();
    }

    public async Task<List<RiderProfileDto>> GetFollowingAsync(Guid userId, Guid? currentUserId)
    {
        var following = await _context.UserFollows.AsNoTracking()
            .Include(f => f.Following)
            .Where(f => f.FollowerId == userId)
            .Select(f => f.Following)
            .ToListAsync();

        return following.Select(u => new RiderProfileDto(
            u.Id, u.FirstName, u.LastName, u.ProfilePhotoUrl, u.Province,
            u.TotalDistanceKm, u.TotalEventsCompleted, u.TotalPoints,
            null, null, u.CreatedAt, 0, 0, false, new(), new(), new()
        )).ToList();
    }
}
