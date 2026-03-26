using Lakbay.Application.DTOs.Common;
using Lakbay.Application.DTOs.Events;

namespace Lakbay.Application.Interfaces;

public interface IEventService
{
    Task<PagedResult<EventDto>> GetEventsAsync(
        int page, int pageSize,
        string? sortField = null, string? sortOrder = null,
        string? search = null, string? eventType = null,
        string? difficulty = null, string? region = null,
        string? status = null);
    
    Task<EventDetailDto?> GetEventByIdAsync(Guid id);
    Task<EventDto> CreateEventAsync(CreateEventRequest request, string createdBy);
    Task<EventDto?> UpdateEventAsync(Guid id, UpdateEventRequest request, string updatedBy);
    Task<bool> DeleteEventAsync(Guid id);
    Task<bool> UpdateEventStatusAsync(Guid id, int statusId, string updatedBy);
    
    // Registration
    Task<(EventRegistrationDto? Registration, string? ErrorMessage)> RegisterForEventAsync(Guid eventId, Guid userId, RegisterForEventRequest request);
    Task<bool> CancelRegistrationAsync(Guid eventId, Guid userId);
    Task<PagedResult<EventRegistrationDto>> GetUserRegistrationsAsync(Guid userId, int page, int pageSize);
    Task<EventRegistrationDto?> CompleteRegistrationAsync(Guid eventId, Guid userId, decimal? distanceKm, TimeSpan? completionTime, string updatedBy);
}
