namespace Piloopinas.Domain.Entities;

public class Badge
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IconUrl { get; set; }
    public string Category { get; set; } = string.Empty; // Distance, Events, Streak, Special
    
    // Achievement criteria
    public string CriteriaType { get; set; } = string.Empty; // DistanceReached, EventsCompleted, ConsecutiveDays, etc.
    public int? CriteriaValue { get; set; } // e.g., 1000 for 1000km badge
    public int PointsValue { get; set; }
    
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    
    public ICollection<UserBadge> UserBadges { get; set; } = new List<UserBadge>();
}
