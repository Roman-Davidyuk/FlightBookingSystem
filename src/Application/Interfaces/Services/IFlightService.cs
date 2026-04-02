using Application.DTOs;

namespace Application.Interfaces.Services;

public interface IFlightService
{
    Task<IEnumerable<FlightDto>> SearchFlightsAsync(string? origin, string? destination, DateTime? date, CancellationToken ct);
    Task<FlightDto> GetByIdAsync(Guid id, CancellationToken ct);
}