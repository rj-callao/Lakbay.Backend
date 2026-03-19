using Microsoft.EntityFrameworkCore;
using Piloopinas.Application.DTOs.Users;
using Piloopinas.Application.Interfaces;
using Piloopinas.Domain.Entities;
using Piloopinas.Infrastructure.Data;

namespace Piloopinas.Infrastructure.Services;

public class MotorcycleService : IMotorcycleService
{
    private readonly AppDbContext _context;

    public MotorcycleService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<MotorcycleDto>> GetUserMotorcyclesAsync(Guid userId)
    {
        return await _context.Motorcycles
            .AsNoTracking()
            .Where(m => m.UserId == userId && m.IsActive)
            .OrderByDescending(m => m.IsPrimary)
            .ThenByDescending(m => m.CreatedAt)
            .Select(m => new MotorcycleDto(
                m.Id,
                m.Brand,
                m.Model,
                m.Year,
                m.PlateNumber,
                m.Color,
                m.EngineDisplacementCc,
                m.PhotoUrl,
                m.IsPrimary
            ))
            .ToListAsync();
    }

    public async Task<MotorcycleDto?> GetMotorcycleByIdAsync(Guid id, Guid userId)
    {
        var motorcycle = await _context.Motorcycles
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId && m.IsActive);

        if (motorcycle == null) return null;

        return new MotorcycleDto(
            motorcycle.Id,
            motorcycle.Brand,
            motorcycle.Model,
            motorcycle.Year,
            motorcycle.PlateNumber,
            motorcycle.Color,
            motorcycle.EngineDisplacementCc,
            motorcycle.PhotoUrl,
            motorcycle.IsPrimary
        );
    }

    public async Task<MotorcycleDto> CreateMotorcycleAsync(Guid userId, CreateMotorcycleRequest request)
    {
        // If this is the first motorcycle or set as primary, update other motorcycles
        if (request.IsPrimary)
        {
            var existingPrimary = await _context.Motorcycles
                .Where(m => m.UserId == userId && m.IsPrimary && m.IsActive)
                .ToListAsync();

            foreach (var m in existingPrimary)
            {
                m.IsPrimary = false;
            }
        }

        // Check if this is the first motorcycle
        var hasMotorcycles = await _context.Motorcycles
            .AnyAsync(m => m.UserId == userId && m.IsActive);

        var motorcycle = new Motorcycle
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Brand = request.Brand,
            Model = request.Model,
            Year = request.Year,
            PlateNumber = request.PlateNumber,
            Color = request.Color,
            EngineDisplacementCc = request.EngineDisplacementCc,
            PhotoUrl = request.PhotoUrl,
            IsPrimary = request.IsPrimary || !hasMotorcycles, // First motorcycle is always primary
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId.ToString()
        };

        _context.Motorcycles.Add(motorcycle);
        await _context.SaveChangesAsync();

        return new MotorcycleDto(
            motorcycle.Id,
            motorcycle.Brand,
            motorcycle.Model,
            motorcycle.Year,
            motorcycle.PlateNumber,
            motorcycle.Color,
            motorcycle.EngineDisplacementCc,
            motorcycle.PhotoUrl,
            motorcycle.IsPrimary
        );
    }

    public async Task<MotorcycleDto?> UpdateMotorcycleAsync(Guid id, Guid userId, UpdateMotorcycleRequest request)
    {
        var motorcycle = await _context.Motorcycles
            .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId && m.IsActive);

        if (motorcycle == null) return null;

        // If setting as primary, remove primary from others
        if (request.IsPrimary && !motorcycle.IsPrimary)
        {
            var existingPrimary = await _context.Motorcycles
                .Where(m => m.UserId == userId && m.IsPrimary && m.IsActive && m.Id != id)
                .ToListAsync();

            foreach (var m in existingPrimary)
            {
                m.IsPrimary = false;
            }
        }

        motorcycle.Brand = request.Brand;
        motorcycle.Model = request.Model;
        motorcycle.Year = request.Year;
        motorcycle.PlateNumber = request.PlateNumber;
        motorcycle.Color = request.Color;
        motorcycle.EngineDisplacementCc = request.EngineDisplacementCc;
        motorcycle.PhotoUrl = request.PhotoUrl;
        motorcycle.IsPrimary = request.IsPrimary;
        motorcycle.UpdatedAt = DateTime.UtcNow;
        motorcycle.UpdatedBy = userId.ToString();

        await _context.SaveChangesAsync();

        return new MotorcycleDto(
            motorcycle.Id,
            motorcycle.Brand,
            motorcycle.Model,
            motorcycle.Year,
            motorcycle.PlateNumber,
            motorcycle.Color,
            motorcycle.EngineDisplacementCc,
            motorcycle.PhotoUrl,
            motorcycle.IsPrimary
        );
    }

    public async Task<bool> DeleteMotorcycleAsync(Guid id, Guid userId)
    {
        var motorcycle = await _context.Motorcycles
            .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId && m.IsActive);

        if (motorcycle == null) return false;

        motorcycle.IsActive = false;
        motorcycle.UpdatedAt = DateTime.UtcNow;
        motorcycle.UpdatedBy = userId.ToString();

        // If this was primary, set another as primary
        if (motorcycle.IsPrimary)
        {
            var nextMotorcycle = await _context.Motorcycles
                .Where(m => m.UserId == userId && m.IsActive && m.Id != id)
                .OrderByDescending(m => m.CreatedAt)
                .FirstOrDefaultAsync();

            if (nextMotorcycle != null)
            {
                nextMotorcycle.IsPrimary = true;
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SetPrimaryMotorcycleAsync(Guid id, Guid userId)
    {
        var motorcycle = await _context.Motorcycles
            .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId && m.IsActive);

        if (motorcycle == null) return false;

        // Remove primary from others
        var existingPrimary = await _context.Motorcycles
            .Where(m => m.UserId == userId && m.IsPrimary && m.IsActive && m.Id != id)
            .ToListAsync();

        foreach (var m in existingPrimary)
        {
            m.IsPrimary = false;
        }

        motorcycle.IsPrimary = true;
        motorcycle.UpdatedAt = DateTime.UtcNow;
        motorcycle.UpdatedBy = userId.ToString();

        await _context.SaveChangesAsync();
        return true;
    }
}
