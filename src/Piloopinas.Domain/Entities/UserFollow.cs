namespace Lakbay.Domain.Entities;

public class UserFollow
{
    public Guid Id { get; set; }
    public Guid FollowerId { get; set; }
    public User Follower { get; set; } = null!;
    public Guid FollowingId { get; set; }
    public User Following { get; set; } = null!;
    public DateTime FollowedAt { get; set; }
}
