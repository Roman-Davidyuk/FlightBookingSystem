using Core.Entities;
using FluentAssertions;
using Xunit;

namespace Unit.Entities;

public class FlightTests
{
    private Passenger CreatePassenger() => new Passenger("John", "Doe", "john@example.com", "AB123456");

    [Fact]
    public void AddBooking_WhenSeatsAvailable_ShouldDecreaseAvailableSeats()
    {
        var flight = new Flight("FL-123", "Kyiv", "London", DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(1).AddHours(3), 10, 100m);
        flight.AddBooking(CreatePassenger(), "1A");
        flight.AvailableSeats.Should().Be(9);
    }

    [Fact]
    public void AddBooking_ShouldAddBookingToCollection()
    {
        var flight = new Flight("FL-123", "Kyiv", "London", DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(1).AddHours(3), 10, 100m);
        var booking = flight.AddBooking(CreatePassenger(), "1A");
        flight.Bookings.Should().Contain(booking);
    }

    [Fact]
    public void AddBooking_WhenNoSeatsAvailable_ShouldThrowInvalidOperationException()
    {
        var flight = new Flight("FL-123", "Kyiv", "London", DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(1).AddHours(3), 1, 100m);
        flight.AddBooking(CreatePassenger(), "1A"); 

        Action act = () => flight.AddBooking(new Passenger("Jane", "Doe", "j@j.com", "CD987654"), "1B");
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void AddBooking_DuplicateSeatNumber_ShouldThrowInvalidOperationException()
    {
        var flight = new Flight("FL-123", "Kyiv", "London", DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(1).AddHours(3), 10, 100m);
        flight.AddBooking(CreatePassenger(), "12A"); 
        
        Action act = () => flight.AddBooking(new Passenger("Jane", "Doe", "j@j.com", "CD987654"), "12A");
        act.Should().Throw<InvalidOperationException>().WithMessage("*already booked*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void AddBooking_EmptySeatNumber_ShouldThrowArgumentException(string invalidSeat)
    {
        var flight = new Flight("FL-123", "Kyiv", "London", DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(1).AddHours(3), 10, 100m);
        Action act = () => flight.AddBooking(CreatePassenger(), invalidSeat);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void AddBooking_NullPassenger_ShouldThrowArgumentNullException()
    {
        var flight = new Flight("FL-123", "Kyiv", "London", DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(1).AddHours(3), 10, 100m);
        Action act = () => flight.AddBooking(null!, "1A");
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CancelBooking_ShouldIncreaseAvailableSeatsAndRemoveBooking()
    {
        var flight = new Flight("FL-123", "Kyiv", "London", DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(2).AddHours(3), 10, 100m);
        var booking = flight.AddBooking(CreatePassenger(), "1A");
        
        flight.CancelBooking(booking, DateTime.UtcNow);

        flight.AvailableSeats.Should().Be(10);
        booking.Status.Should().Be(BookingStatus.Cancelled);
    }
}