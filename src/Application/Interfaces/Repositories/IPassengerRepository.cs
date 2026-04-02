using Core.Entities;

namespace Application.Interfaces.Repositories;

public interface IPassengerRepository
{
    Task<Passenger?> GetByIdAsync(Guid id, CancellationToken ct);
}