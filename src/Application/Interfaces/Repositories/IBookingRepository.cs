using Core.Entities;

namespace Application.Interfaces.Repositories;

public interface IBookingRepository
{
    Task<IEnumerable<Booking>> GetAllAsync(CancellationToken ct);
    Task AddAsync(Booking booking, CancellationToken ct);
    Task<Booking?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<Booking?> GetByIdWithFlightAsync(Guid id, CancellationToken ct);
    Task<IEnumerable<Booking>> GetByPassengerIdAsync(Guid passengerId, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}