using Application.Interfaces.Repositories;
using Core.Entities;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

public class PassengerRepository : IPassengerRepository
{
    private readonly AppDbContext _context;

    public PassengerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Passenger?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _context.Passengers.FindAsync(new object[] { id }, ct);
    }
}