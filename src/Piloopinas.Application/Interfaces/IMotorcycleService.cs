using Piloopinas.Application.DTOs.Users;

namespace Piloopinas.Application.Interfaces;

public interface IMotorcycleService
{
    Task<List<MotorcycleDto>> GetUserMotorcyclesAsync(Guid userId);
    Task<MotorcycleDto?> GetMotorcycleByIdAsync(Guid id, Guid userId);
    Task<MotorcycleDto> CreateMotorcycleAsync(Guid userId, CreateMotorcycleRequest request);
    Task<MotorcycleDto?> UpdateMotorcycleAsync(Guid id, Guid userId, UpdateMotorcycleRequest request);
    Task<bool> DeleteMotorcycleAsync(Guid id, Guid userId);
    Task<bool> SetPrimaryMotorcycleAsync(Guid id, Guid userId);
}
