using Application.DTOs;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FlightsController : ControllerBase
{
    private readonly IFlightService _flightService;

    public FlightsController(IFlightService flightService)
    {
        _flightService = flightService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FlightDto>>> SearchFlights(
        [FromQuery] string? origin, 
        [FromQuery] string? destination, 
        [FromQuery] DateTime? date,
        CancellationToken ct)
    {
        var flights = await _flightService.SearchFlightsAsync(origin, destination, date, ct);
        return Ok(flights);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<FlightDto>> GetFlight(Guid id, CancellationToken ct)
    {
        try
        {
            var flight = await _flightService.GetByIdAsync(id, ct);
            return Ok(flight);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}