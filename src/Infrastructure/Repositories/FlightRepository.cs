using Application.Interfaces.Repositories;
using Core.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class FlightRepository : IFlightRepository
{
    private readonly AppDbContext _context;

    public FlightRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Flight>> SearchAsync(string? origin, string? destination, DateTime? date, CancellationToken ct)
    {
        var query = _context.Flights.AsQueryable();

        if (!string.IsNullOrWhiteSpace(origin))
            query = query.Where(f => f.Origin.Contains(origin));
        
        if (!string.IsNullOrWhiteSpace(destination))
            query = query.Where(f => f.Destination.Contains(destination));

        if (date.HasValue)
            query = query.Where(f => f.DepartureTime.Date == date.Value.Date);

        return await query.ToListAsync(ct);
    }

    public async Task<Flight?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _context.Flights.FindAsync(new object[] { id }, ct);
    }
}