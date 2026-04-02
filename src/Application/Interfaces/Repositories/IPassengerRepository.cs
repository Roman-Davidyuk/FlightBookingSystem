using Core.Entities;

namespace Application.Interfaces.Repositories;

public interface IPassengerRepository
{
    Task<IEnumerable<Passenger>> GetAllAsync(CancellationToken ct);
    Task<Passenger?> GetByIdAsync(Guid id, CancellationToken ct);
}