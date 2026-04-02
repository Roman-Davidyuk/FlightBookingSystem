using Core.Entities;

namespace Application.Interfaces.Repositories;

public interface IFlightRepository
{
    Task<IEnumerable<Flight>> SearchAsync(string? origin, string? destination, DateTime? date, CancellationToken ct);
    Task<Flight?> GetByIdAsync(Guid id, CancellationToken ct);
}