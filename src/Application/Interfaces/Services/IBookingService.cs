using Application.DTOs;

namespace Application.Interfaces.Services;

public interface IBookingService
{
    Task<IEnumerable<BookingDto>> GetAllBookingsAsync(CancellationToken ct);
    Task<BookingDto> BookFlightAsync(CreateBookingDto dto, CancellationToken ct);
    Task<BookingDto> GetBookingAsync(Guid id, CancellationToken ct);
    Task CancelBookingAsync(Guid id, CancellationToken ct);
    Task BoardPassengerAsync(Guid id, CancellationToken ct);
    Task<IEnumerable<BookingDto>> GetPassengerBookingsAsync(Guid passengerId, CancellationToken ct);
}