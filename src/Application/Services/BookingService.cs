using Application.DTOs;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;

namespace Application.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IFlightRepository _flightRepository;
    private readonly IPassengerRepository _passengerRepository;

    public BookingService(IBookingRepository bookingRepo, IFlightRepository flightRepo, IPassengerRepository passengerRepo)
    {
        _bookingRepository = bookingRepo;
        _flightRepository = flightRepo;
        _passengerRepository = passengerRepo;
    }
    public async Task<IEnumerable<BookingDto>> GetAllBookingsAsync(CancellationToken ct)
    {
        var bookings = await _bookingRepository.GetAllAsync(ct);
        return bookings.Select(MapToDto);
    }

    public async Task<BookingDto> BookFlightAsync(CreateBookingDto dto, CancellationToken ct)
    {
        var flight = await _flightRepository.GetByIdAsync(dto.FlightId, ct);
        if (flight == null) throw new KeyNotFoundException("Flight not found.");

        var passenger = await _passengerRepository.GetByIdAsync(dto.PassengerId, ct);
        if (passenger == null) throw new KeyNotFoundException("Passenger not found.");

        var booking = flight.AddBooking(passenger, dto.SeatNumber);
        
        await _bookingRepository.AddAsync(booking, ct);

        try
        {
            await _bookingRepository.SaveChangesAsync(ct);
        }
        catch (Exception ex) when (ex.InnerException?.Message.Contains("UNIQUE constraint") == true || ex.Message.Contains("duplicate"))
        {
            throw new InvalidOperationException($"Seat {dto.SeatNumber} is already booked on this flight.");
        }

        return MapToDto(booking);
    }

    public async Task<BookingDto> GetBookingAsync(Guid id, CancellationToken ct)
    {
        var booking = await _bookingRepository.GetByIdAsync(id, ct);
        if (booking == null) throw new KeyNotFoundException("Booking not found.");
        return MapToDto(booking);
    }

    public async Task CancelBookingAsync(Guid id, CancellationToken ct)
    {
        var booking = await _bookingRepository.GetByIdWithFlightAsync(id, ct);
        if (booking == null || booking.Flight == null) throw new KeyNotFoundException("Booking or Flight not found.");

        booking.Flight.CancelBooking(booking, DateTime.UtcNow);
        await _bookingRepository.SaveChangesAsync(ct);
    }

    public async Task BoardPassengerAsync(Guid id, CancellationToken ct)
    {
        var booking = await _bookingRepository.GetByIdWithFlightAsync(id, ct);
        if (booking == null || booking.Flight == null) throw new KeyNotFoundException("Booking or Flight not found.");

        booking.Board(DateTime.UtcNow, booking.Flight);
        await _bookingRepository.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<BookingDto>> GetPassengerBookingsAsync(Guid passengerId, CancellationToken ct)
    {
        var bookings = await _bookingRepository.GetByPassengerIdAsync(passengerId, ct);
        return bookings.Select(MapToDto);
    }

    private static BookingDto MapToDto(Core.Entities.Booking booking)
    {
        return new BookingDto
        {
            Id = booking.Id,
            FlightId = booking.FlightId,
            PassengerId = booking.PassengerId,
            SeatNumber = booking.SeatNumber,
            BookingDate = booking.BookingDate,
            Status = booking.Status.ToString()
        };
    }
}