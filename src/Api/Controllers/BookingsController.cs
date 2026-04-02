using Application.DTOs;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetAllBookings(CancellationToken ct)
    {
        var bookings = await _bookingService.GetAllBookingsAsync(ct);
        return Ok(bookings);
    }

    [HttpPost]
    public async Task<ActionResult<BookingDto>> BookFlight([FromBody] CreateBookingDto dto, CancellationToken ct)
    {
        try
        {
            var booking = await _bookingService.BookFlightAsync(dto, ct);
            return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, booking);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BookingDto>> GetBooking(Guid id, CancellationToken ct)
    {
        try
        {
            var booking = await _bookingService.GetBookingAsync(id, ct);
            return Ok(booking);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> CancelBooking(Guid id, CancellationToken ct)
    {
        try
        {
            await _bookingService.CancelBookingAsync(id, ct);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id:guid}/board")]
    public async Task<IActionResult> BoardPassenger(Guid id, CancellationToken ct)
    {
        try
        {
            await _bookingService.BoardPassengerAsync(id, ct);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}