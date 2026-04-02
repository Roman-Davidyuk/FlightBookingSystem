using Application.Interfaces.Repositories;
using Core.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly AppDbContext _context;

    public BookingRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<Booking>> GetAllAsync(CancellationToken ct)
    {
        return await _context.Bookings.ToListAsync(ct);
    }

    public async Task AddAsync(Booking booking, CancellationToken ct)
    {
        await _context.Bookings.AddAsync(booking, ct);
    }

    public async Task<Booking?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _context.Bookings.FindAsync(new object[] { id }, ct);
    }

    public async Task<Booking?> GetByIdWithFlightAsync(Guid id, CancellationToken ct)
    {
        return await _context.Bookings
            .Include(b => b.Flight)
            .FirstOrDefaultAsync(b => b.Id == id, ct);
    }

    public async Task<IEnumerable<Booking>> GetByPassengerIdAsync(Guid passengerId, CancellationToken ct)
    {
        return await _context.Bookings
            .Where(b => b.PassengerId == passengerId)
            .ToListAsync(ct);
    }

    public async Task SaveChangesAsync(CancellationToken ct)
    {
        await _context.SaveChangesAsync(ct);
    }
}