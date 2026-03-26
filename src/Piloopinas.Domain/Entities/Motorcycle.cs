namespace Lakbay.Domain.Entities;

public class Motorcycle
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int? Year { get; set; }
    public string? PlateNumber { get; set; }
    public string? Color { get; set; }
    public int? EngineDisplacementCc { get; set; }
    public string? PhotoUrl { get; set; }
    
    public bool IsPrimary { get; set; }
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
