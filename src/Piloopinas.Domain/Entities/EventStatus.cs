namespace Piloopinas.Domain.Entities;

public class EventStatus
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    public ICollection<Event> Events { get; set; } = new List<Event>();
}
