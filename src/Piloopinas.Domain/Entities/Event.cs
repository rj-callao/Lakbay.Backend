namespace Piloopinas.Domain.Entities;

public class Event
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    
    // Event details
    public string EventType { get; set; } = string.Empty; // Endurance, Loop, Tour, etc.
    public string Difficulty { get; set; } = string.Empty; // Easy, Moderate, Hard, Extreme
    public decimal DistanceKm { get; set; }
    public string? Route { get; set; } // Route description or encoded path
    
    // Location
    public string StartLocation { get; set; } = string.Empty;
    public string? EndLocation { get; set; }
    public string? Region { get; set; } // Luzon, Visayas, Mindanao
    public string? Province { get; set; }
    
    // Schedule
    public DateTime StartDateTime { get; set; }
    public DateTime? EndDateTime { get; set; }
    public DateTime RegistrationDeadline { get; set; }
    
    // Capacity and fees
    public int MaxParticipants { get; set; }
    public decimal RegistrationFee { get; set; }
    public int PointsReward { get; set; }
    
    // Status
    public int StatusId { get; set; }
    public EventStatus Status { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    
    // Navigation properties
    public ICollection<EventRegistration> Registrations { get; set; } = new List<EventRegistration>();
}
