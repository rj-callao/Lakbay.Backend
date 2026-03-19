using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piloopinas.Application.DTOs.Events;
using Piloopinas.Application.Interfaces;

namespace Piloopinas.Api.Controllers;

[Route("api/v1/events")]
[Authorize]
public class EventsController : AppBaseController
{
    private readonly IEventService _eventService;

    public EventsController(IEventService eventService)
    {
        _eventService = eventService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetEvents(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        [FromQuery] string? sortField = null,
        [FromQuery] string? sortOrder = null,
        [FromQuery] string? search = null,
        [FromQuery] string? eventType = null,
        [FromQuery] string? difficulty = null,
        [FromQuery] string? region = null,
        [FromQuery] string? status = null)
    {
        var result = await _eventService.GetEventsAsync(
            page, pageSize, sortField, sortOrder,
            search, eventType, difficulty, region, status);
        
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetEvent(Guid id)
    {
        var result = await _eventService.GetEventByIdAsync(id);
        
        if (result == null)
            return NotFound(new { error = new { message = "Event not found" } });
        
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Organizer")]
    public async Task<IActionResult> CreateEvent([FromBody] CreateEventRequest request)
    {
        var result = await _eventService.CreateEventAsync(request, GetCurrentUserId().ToString());
        return CreatedAtAction(nameof(GetEvent), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Organizer")]
    public async Task<IActionResult> UpdateEvent(Guid id, [FromBody] UpdateEventRequest request)
    {
        var result = await _eventService.UpdateEventAsync(id, request, GetCurrentUserId().ToString());
        
        if (result == null)
            return NotFound(new { error = new { message = "Event not found" } });
        
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteEvent(Guid id)
    {
        var result = await _eventService.DeleteEventAsync(id);
        
        if (!result)
            return NotFound(new { error = new { message = "Event not found" } });
        
        return NoContent();
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = "Admin,Organizer")]
    public async Task<IActionResult> UpdateEventStatus(Guid id, [FromBody] int statusId)
    {
        var result = await _eventService.UpdateEventStatusAsync(id, statusId, GetCurrentUserId().ToString());
        
        if (!result)
            return NotFound(new { error = new { message = "Event not found" } });
        
        return Ok(new { message = "Status updated successfully" });
    }

    [HttpPost("{id:guid}/register")]
    public async Task<IActionResult> RegisterForEvent(Guid id, [FromBody] RegisterForEventRequest? request)
    {
        var userId = GetCurrentUserId();
        var (registration, errorMessage) = await _eventService.RegisterForEventAsync(id, userId, request ?? new RegisterForEventRequest(null));
        
        if (registration == null)
            return BadRequest(new { error = new { message = errorMessage ?? "Unable to register." } });
        
        return Ok(registration);
    }

    [HttpDelete("{id:guid}/register")]
    public async Task<IActionResult> CancelRegistration(Guid id)
    {
        var userId = GetCurrentUserId();
        var result = await _eventService.CancelRegistrationAsync(id, userId);
        
        if (!result)
            return NotFound(new { error = new { message = "Registration not found" } });
        
        return Ok(new { message = "Registration cancelled successfully" });
    }

    [HttpGet("my-registrations")]
    public async Task<IActionResult> GetMyRegistrations(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25)
    {
        var userId = GetCurrentUserId();
        var result = await _eventService.GetUserRegistrationsAsync(userId, page, pageSize);
        return Ok(result);
    }

    [HttpPost("{eventId:guid}/complete/{userId:guid}")]
    [Authorize(Roles = "Admin,Organizer")]
    public async Task<IActionResult> CompleteRegistration(
        Guid eventId,
        Guid userId,
        [FromQuery] decimal? distanceKm = null,
        [FromQuery] string? completionTime = null)
    {
        TimeSpan? time = null;
        if (!string.IsNullOrEmpty(completionTime) && TimeSpan.TryParse(completionTime, out var parsedTime))
        {
            time = parsedTime;
        }

        var result = await _eventService.CompleteRegistrationAsync(
            eventId, userId, distanceKm, time, GetCurrentUserId().ToString());
        
        if (result == null)
            return NotFound(new { error = new { message = "Registration not found" } });
        
        return Ok(result);
    }
}
