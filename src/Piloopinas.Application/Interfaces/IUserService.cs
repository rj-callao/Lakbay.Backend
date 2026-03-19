using Piloopinas.Application.DTOs.Common;
using Piloopinas.Application.DTOs.Users;

namespace Piloopinas.Application.Interfaces;

public interface IUserService
{
    Task<PagedResult<AdminUserDto>> GetUsersAsync(
        int page, int pageSize,
        string? sortField = null, string? sortOrder = null,
        string? search = null, string? role = null,
        bool? isActive = null);
    
    Task<UserProfileDto?> GetUserByIdAsync(Guid id);
    Task<UserProfileDto?> GetUserProfileAsync(Guid userId);
    Task<AdminUserDto> CreateUserAsync(CreateUserRequest request, string createdBy);
    Task<AdminUserDto?> UpdateUserAsync(Guid id, UpdateUserRequest request, string updatedBy);
    Task<bool> DeleteUserAsync(Guid id);
    Task<bool> UpdateProfileAsync(Guid userId, UpdateProfileRequest request);
    
    // Public rider profiles & follows
    Task<RiderProfileDto?> GetRiderProfileAsync(Guid riderId, Guid? currentUserId);
    Task<FollowResultDto?> ToggleFollowAsync(Guid followerId, Guid followingId);
    Task<List<RiderProfileDto>> GetFollowersAsync(Guid userId, Guid? currentUserId);
    Task<List<RiderProfileDto>> GetFollowingAsync(Guid userId, Guid? currentUserId);
}
