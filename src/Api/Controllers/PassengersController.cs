using Application.DTOs;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PassengersController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly IPassengerService _passengerService;

    public PassengersController(IBookingService bookingService, IPassengerService passengerService)
    {
        _bookingService = bookingService;
        _passengerService = passengerService;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PassengerDto>>> GetAllPassengers(CancellationToken ct)
    {
        var passengers = await _passengerService.GetAllPassengersAsync(ct);
        return Ok(passengers);
    }
    
    [HttpGet("{id:guid}/bookings")]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetPassengerBookings(Guid id, CancellationToken ct)
    {
        var bookings = await _bookingService.GetPassengerBookingsAsync(id, ct);
        return Ok(bookings);
    }
}