using Application.DTOs;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;

namespace Application.Services;

public class FlightService : IFlightService
{
    private readonly IFlightRepository _flightRepository;

    public FlightService(IFlightRepository flightRepository)
    {
        _flightRepository = flightRepository;
    }

    public async Task<IEnumerable<FlightDto>> SearchFlightsAsync(string? origin, string? destination, DateTime? date, CancellationToken ct)
    {
        var flights = await _flightRepository.SearchAsync(origin, destination, date, ct);

        return flights.Select(f => new FlightDto
        {
            Id = f.Id,
            FlightNumber = f.FlightNumber,
            Origin = f.Origin,
            Destination = f.Destination,
            DepartureTime = f.DepartureTime,
            ArrivalTime = f.ArrivalTime,
            TotalSeats = f.TotalSeats,
            AvailableSeats = f.AvailableSeats,
            Price = f.Price
        });
    }

    public async Task<FlightDto> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var flight = await _flightRepository.GetByIdAsync(id, ct);
        if (flight == null) throw new KeyNotFoundException("Flight not found.");

        return new FlightDto
        {
            Id = flight.Id,
            FlightNumber = flight.FlightNumber,
            Origin = flight.Origin,
            Destination = flight.Destination,
            DepartureTime = flight.DepartureTime,
            ArrivalTime = flight.ArrivalTime,
            TotalSeats = flight.TotalSeats,
            AvailableSeats = flight.AvailableSeats,
            Price = flight.Price
        };
    }
}