using Microsoft.EntityFrameworkCore;
using Lakbay.Application.DTOs.Common;
using Lakbay.Application.DTOs.Events;
using Lakbay.Application.Interfaces;
using Lakbay.Domain;
using Lakbay.Domain.Entities;
using Lakbay.Infrastructure.Data;

namespace Lakbay.Infrastructure.Services;

public class EventService : IEventService
{
    private readonly AppDbContext _context;

    public EventService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<EventDto>> GetEventsAsync(
        int page, int pageSize,
        string? sortField = null, string? sortOrder = null,
        string? search = null, string? eventType = null,
        string? difficulty = null, string? region = null,
        string? status = null)
    {
        var query = _context.Events
            .AsNoTracking()
            .Include(e => e.Status)
            .Include(e => e.Registrations)
            .AsQueryable();

        // Apply filters
        if (!string.IsNullOrEmpty(search))
            query = query.Where(e => e.Name.Contains(search) || 
                                     (e.Description != null && e.Description.Contains(search)));

        if (!string.IsNullOrEmpty(eventType))
        {
            var types = eventType.Split(',', StringSplitOptions.RemoveEmptyEntries);
            query = query.Where(e => types.Contains(e.EventType));
        }

        if (!string.IsNullOrEmpty(difficulty))
        {
            var difficulties = difficulty.Split(',', StringSplitOptions.RemoveEmptyEntries);
            query = query.Where(e => difficulties.Contains(e.Difficulty));
        }

        if (!string.IsNullOrEmpty(region))
        {
            var regions = region.Split(',', StringSplitOptions.RemoveEmptyEntries);
            query = query.Where(e => e.Region != null && regions.Contains(e.Region));
        }

        if (!string.IsNullOrEmpty(status))
        {
            var statuses = status.Split(',', StringSplitOptions.RemoveEmptyEntries);
            query = query.Where(e => statuses.Contains(e.Status.Name));
        }

        var totalCount = await query.CountAsync();

        // Apply sorting
        query = (sortField?.ToLower(), sortOrder?.ToLower()) switch
        {
            ("name", "asc") => query.OrderBy(e => e.Name),
            ("name", _) => query.OrderByDescending(e => e.Name),
            ("startdatetime", "asc") => query.OrderBy(e => e.StartDateTime),
            ("startdatetime", _) => query.OrderByDescending(e => e.StartDateTime),
            ("distancekm", "asc") => query.OrderBy(e => e.DistanceKm),
            ("distancekm", _) => query.OrderByDescending(e => e.DistanceKm),
            ("registrationfee", "asc") => query.OrderBy(e => e.RegistrationFee),
            ("registrationfee", _) => query.OrderByDescending(e => e.RegistrationFee),
            _ => query.OrderByDescending(e => e.StartDateTime)
        };

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(e => new EventDto(
                e.Id,
                e.Name,
                e.Description,
                e.ImageUrl,
                e.EventType,
                e.Difficulty,
                e.DistanceKm,
                e.StartLocation,
                e.EndLocation,
                e.Region,
                e.Province,
                e.StartDateTime,
                e.EndDateTime,
                e.RegistrationDeadline,
                e.MaxParticipants,
                e.Registrations.Count(r => r.RegistrationStatus != "Cancelled"),
                e.RegistrationFee,
                e.PointsReward,
                e.Status.Name,
                e.CreatedAt
            ))
            .ToListAsync();

        return new PagedResult<EventDto>(items, totalCount, page, pageSize);
    }

    public async Task<EventDetailDto?> GetEventByIdAsync(Guid id)
    {
        var e = await _context.Events
            .AsNoTracking()
            .Include(ev => ev.Status)
            .Include(ev => ev.Registrations)
                .ThenInclude(r => r.User)
            .Include(ev => ev.Registrations)
                .ThenInclude(r => r.Motorcycle)
            .FirstOrDefaultAsync(ev => ev.Id == id);

        if (e == null) return null;

        return new EventDetailDto(
            e.Id,
            e.Name,
            e.Description,
            e.ImageUrl,
            e.EventType,
            e.Difficulty,
            e.DistanceKm,
            e.Route,
            e.StartLocation,
            e.EndLocation,
            e.Region,
            e.Province,
            e.StartDateTime,
            e.EndDateTime,
            e.RegistrationDeadline,
            e.MaxParticipants,
            e.Registrations.Count(r => r.RegistrationStatus != "Cancelled"),
            e.RegistrationFee,
            e.PointsReward,
            e.Status.Name,
            e.CreatedAt,
            e.Registrations
                .Where(r => r.RegistrationStatus != "Cancelled")
                .Select(r => new EventParticipantDto(
                    r.UserId,
                    $"{r.User.FirstName} {r.User.LastName}",
                    r.User.ProfilePhotoUrl,
                    r.Motorcycle != null ? $"{r.Motorcycle.Brand} {r.Motorcycle.Model}" : null,
                    r.RegistrationStatus,
                    r.RegisteredAt,
                    r.IsCompleted,
                    r.FinishPosition
                ))
                .ToList()
        );
    }

    public async Task<EventDto> CreateEventAsync(CreateEventRequest request, string createdBy)
    {
        var eventEntity = new Event
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            ImageUrl = request.ImageUrl,
            EventType = request.EventType,
            Difficulty = request.Difficulty,
            DistanceKm = request.DistanceKm,
            Route = request.Route,
            StartLocation = request.StartLocation,
            EndLocation = request.EndLocation,
            Region = request.Region,
            Province = request.Province,
            StartDateTime = request.StartDateTime,
            EndDateTime = request.EndDateTime,
            RegistrationDeadline = request.RegistrationDeadline,
            MaxParticipants = request.MaxParticipants,
            RegistrationFee = request.RegistrationFee,
            PointsReward = request.PointsReward,
            StatusId = EventStatusConstants.Draft,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        _context.Events.Add(eventEntity);
        await _context.SaveChangesAsync();

        return new EventDto(
            eventEntity.Id,
            eventEntity.Name,
            eventEntity.Description,
            eventEntity.ImageUrl,
            eventEntity.EventType,
            eventEntity.Difficulty,
            eventEntity.DistanceKm,
            eventEntity.StartLocation,
            eventEntity.EndLocation,
            eventEntity.Region,
            eventEntity.Province,
            eventEntity.StartDateTime,
            eventEntity.EndDateTime,
            eventEntity.RegistrationDeadline,
            eventEntity.MaxParticipants,
            0,
            eventEntity.RegistrationFee,
            eventEntity.PointsReward,
            "Draft",
            eventEntity.CreatedAt
        );
    }

    public async Task<EventDto?> UpdateEventAsync(Guid id, UpdateEventRequest request, string updatedBy)
    {
        var eventEntity = await _context.Events
            .Include(e => e.Status)
            .Include(e => e.Registrations)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (eventEntity == null) return null;

        eventEntity.Name = request.Name;
        eventEntity.Description = request.Description;
        eventEntity.ImageUrl = request.ImageUrl;
        eventEntity.EventType = request.EventType;
        eventEntity.Difficulty = request.Difficulty;
        eventEntity.DistanceKm = request.DistanceKm;
        eventEntity.Route = request.Route;
        eventEntity.StartLocation = request.StartLocation;
        eventEntity.EndLocation = request.EndLocation;
        eventEntity.Region = request.Region;
        eventEntity.Province = request.Province;
        eventEntity.StartDateTime = request.StartDateTime;
        eventEntity.EndDateTime = request.EndDateTime;
        eventEntity.RegistrationDeadline = request.RegistrationDeadline;
        eventEntity.MaxParticipants = request.MaxParticipants;
        eventEntity.RegistrationFee = request.RegistrationFee;
        eventEntity.PointsReward = request.PointsReward;
        eventEntity.StatusId = request.StatusId;
        eventEntity.UpdatedAt = DateTime.UtcNow;
        eventEntity.UpdatedBy = updatedBy;

        await _context.SaveChangesAsync();

        var status = await _context.EventStatuses.FindAsync(request.StatusId);

        return new EventDto(
            eventEntity.Id,
            eventEntity.Name,
            eventEntity.Description,
            eventEntity.ImageUrl,
            eventEntity.EventType,
            eventEntity.Difficulty,
            eventEntity.DistanceKm,
            eventEntity.StartLocation,
            eventEntity.EndLocation,
            eventEntity.Region,
            eventEntity.Province,
            eventEntity.StartDateTime,
            eventEntity.EndDateTime,
            eventEntity.RegistrationDeadline,
            eventEntity.MaxParticipants,
            eventEntity.Registrations.Count(r => r.RegistrationStatus != "Cancelled"),
            eventEntity.RegistrationFee,
            eventEntity.PointsReward,
            status?.Name ?? "Unknown",
            eventEntity.CreatedAt
        );
    }

    public async Task<bool> DeleteEventAsync(Guid id)
    {
        var eventEntity = await _context.Events.FindAsync(id);
        if (eventEntity == null) return false;

        _context.Events.Remove(eventEntity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateEventStatusAsync(Guid id, int statusId, string updatedBy)
    {
        var eventEntity = await _context.Events.FindAsync(id);
        if (eventEntity == null) return false;

        eventEntity.StatusId = statusId;
        eventEntity.UpdatedAt = DateTime.UtcNow;
        eventEntity.UpdatedBy = updatedBy;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<(EventRegistrationDto? Registration, string? ErrorMessage)> RegisterForEventAsync(Guid eventId, Guid userId, RegisterForEventRequest request)
    {
        var eventEntity = await _context.Events
            .Include(e => e.Status)
            .Include(e => e.Registrations)
            .FirstOrDefaultAsync(e => e.Id == eventId);

        if (eventEntity == null) return (null, "Event not found.");

        // Check if already registered
        if (eventEntity.Registrations.Any(r => r.UserId == userId && r.RegistrationStatus != "Cancelled"))
            return (null, "You are already registered for this event.");

        // Check if event is open for registration
        if (eventEntity.StatusId != EventStatusConstants.RegistrationOpen &&
            eventEntity.StatusId != EventStatusConstants.Published)
            return (null, "Registration is not open for this event.");

        // Check if event is full
        var currentParticipants = eventEntity.Registrations.Count(r => r.RegistrationStatus != "Cancelled");
        if (currentParticipants >= eventEntity.MaxParticipants)
            return (null, "This event is already full.");

        var user = await _context.Users.FindAsync(userId);
        if (user == null) return (null, "User account not found. Please log out and log back in.");

        var isFree = eventEntity.RegistrationFee <= 0;
        var registration = new EventRegistration
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            EventId = eventId,
            MotorcycleId = request.MotorcycleId,
            RegistrationStatus = isFree ? "Confirmed" : "Pending",
            PaymentStatus = isFree ? "N/A" : "Pending",
            AmountPaid = 0,
            RegisteredAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId.ToString()
        };

        _context.EventRegistrations.Add(registration);
        await _context.SaveChangesAsync();

        return (new EventRegistrationDto(
            registration.Id,
            eventId,
            eventEntity.Name,
            userId,
            $"{user.FirstName} {user.LastName}",
            registration.RegistrationStatus,
            registration.PaymentStatus,
            registration.AmountPaid,
            registration.IsCompleted,
            registration.CompletedAt,
            registration.PointsEarned,
            registration.FinishPosition,
            registration.RegisteredAt
        ), null);
    }

    public async Task<bool> CancelRegistrationAsync(Guid eventId, Guid userId)
    {
        var registration = await _context.EventRegistrations
            .FirstOrDefaultAsync(r => r.EventId == eventId && r.UserId == userId);

        if (registration == null) return false;

        registration.RegistrationStatus = "Cancelled";
        registration.UpdatedAt = DateTime.UtcNow;
        registration.UpdatedBy = userId.ToString();

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<PagedResult<EventRegistrationDto>> GetUserRegistrationsAsync(Guid userId, int page, int pageSize)
    {
        var query = _context.EventRegistrations
            .AsNoTracking()
            .Include(r => r.Event)
            .Include(r => r.User)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.RegisteredAt);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(r => new EventRegistrationDto(
                r.Id,
                r.EventId,
                r.Event.Name,
                r.UserId,
                $"{r.User.FirstName} {r.User.LastName}",
                r.RegistrationStatus,
                r.PaymentStatus,
                r.AmountPaid,
                r.IsCompleted,
                r.CompletedAt,
                r.PointsEarned,
                r.FinishPosition,
                r.RegisteredAt
            ))
            .ToListAsync();

        return new PagedResult<EventRegistrationDto>(items, totalCount, page, pageSize);
    }

    public async Task<EventRegistrationDto?> CompleteRegistrationAsync(
        Guid eventId, Guid userId, decimal? distanceKm, TimeSpan? completionTime, string updatedBy)
    {
        var registration = await _context.EventRegistrations
            .Include(r => r.Event)
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.EventId == eventId && r.UserId == userId);

        if (registration == null) return null;

        registration.IsCompleted = true;
        registration.CompletedAt = DateTime.UtcNow;
        registration.ActualDistanceKm = distanceKm ?? registration.Event.DistanceKm;
        registration.CompletionTime = completionTime;
        registration.PointsEarned = registration.Event.PointsReward;
        registration.RegistrationStatus = "Completed";
        registration.UpdatedAt = DateTime.UtcNow;
        registration.UpdatedBy = updatedBy;

        // Update user stats
        registration.User.TotalDistanceKm += registration.ActualDistanceKm ?? 0;
        registration.User.TotalEventsCompleted += 1;
        registration.User.TotalPoints += registration.PointsEarned ?? 0;

        await _context.SaveChangesAsync();

        return new EventRegistrationDto(
            registration.Id,
            registration.EventId,
            registration.Event.Name,
            registration.UserId,
            $"{registration.User.FirstName} {registration.User.LastName}",
            registration.RegistrationStatus,
            registration.PaymentStatus,
            registration.AmountPaid,
            registration.IsCompleted,
            registration.CompletedAt,
            registration.PointsEarned,
            registration.FinishPosition,
            registration.RegisteredAt
        );
    }
}
