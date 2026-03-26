namespace Lakbay.Domain.Entities;

public class EventRegistration
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public Guid EventId { get; set; }
    public Event Event { get; set; } = null!;
    public Guid? MotorcycleId { get; set; }
    public Motorcycle? Motorcycle { get; set; }
    
    public string RegistrationStatus { get; set; } = "Pending"; // Pending, Confirmed, Cancelled, Completed
    public string? PaymentStatus { get; set; } // Pending, Paid, Refunded
    public string? PaymentReference { get; set; }
    public decimal AmountPaid { get; set; }
    
    // Completion data
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public decimal? ActualDistanceKm { get; set; }
    public TimeSpan? CompletionTime { get; set; }
    public int? PointsEarned { get; set; }
    public int? FinishPosition { get; set; }
    
    public DateTime RegisteredAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
