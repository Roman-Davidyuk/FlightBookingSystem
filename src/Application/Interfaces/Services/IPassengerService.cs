using Application.DTOs;

namespace Application.Interfaces.Services;

public interface IPassengerService
{
    Task<IEnumerable<PassengerDto>> GetAllPassengersAsync(CancellationToken ct);
}