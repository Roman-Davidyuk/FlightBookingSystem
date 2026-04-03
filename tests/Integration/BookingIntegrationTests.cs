using System.Net;
using System.Net.Http.Json;
using Application.DTOs;
using Core.Entities;
using FluentAssertions;
using Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IntegrationTests;

[Collection("Integration Tests")]
public class BookingIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public BookingIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private async Task<(Guid FlightId, Guid PassengerId)> GetValidIdsAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        var flight = db.Flights.First();
        var passenger = db.Passengers.First();
        
        return (flight.Id, passenger.Id);
    }

    [Fact]
    public async Task BookFlight_WithValidData_ShouldReturnCreated()
    {
        var (flightId, passengerId) = await GetValidIdsAsync();
        var request = new CreateBookingDto
        {
            FlightId = flightId,
            PassengerId = passengerId,
            SeatNumber = "99Z" 
        };

        var response = await _client.PostAsJsonAsync("/api/bookings", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var booking = await response.Content.ReadFromJsonAsync<BookingDto>();
        booking.Should().NotBeNull();
        booking!.SeatNumber.Should().Be("99Z");
        booking.Status.Should().Be(BookingStatus.Confirmed.ToString());
    }

    [Fact]
    public async Task BookFlight_WithAlreadyBookedSeat_ShouldReturnBadRequest()
    {
        var (flightId, passengerId) = await GetValidIdsAsync();
        var request = new CreateBookingDto
        {
            FlightId = flightId,
            PassengerId = passengerId,
            SeatNumber = "100X" 
        };

        await _client.PostAsJsonAsync("/api/bookings", request);
        var response = await _client.PostAsJsonAsync("/api/bookings", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var error = await response.Content.ReadAsStringAsync();
        error.Should().Contain("is already booked");
    }

    [Fact]
    public async Task CancelBooking_MoreThan24Hours_ShouldReturnNoContent()
    {
        var (flightId, passengerId) = await GetValidIdsAsync();
        
        var createResponse = await _client.PostAsJsonAsync("/api/bookings", new CreateBookingDto
        {
            FlightId = flightId,
            PassengerId = passengerId,
            SeatNumber = "88Y"
        });
        var createdBooking = await createResponse.Content.ReadFromJsonAsync<BookingDto>();

        var cancelResponse = await _client.DeleteAsync($"/api/bookings/{createdBooking!.Id}");

        cancelResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/bookings/{createdBooking.Id}");
        var cancelledBooking = await getResponse.Content.ReadFromJsonAsync<BookingDto>();
        cancelledBooking!.Status.Should().Be(BookingStatus.Cancelled.ToString());
    }
}