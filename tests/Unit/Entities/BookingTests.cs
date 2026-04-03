using Core.Entities;
using FluentAssertions;
using Xunit;

namespace Unit.Entities;

public class BookingTests
{
    private Flight CreateFlight(DateTime departure) => new Flight("FL-1", "K", "L", departure, departure.AddHours(2), 100, 100);
    private Passenger CreatePassenger() => new Passenger("A", "B", "a@b.com", "AB123456");

    [Theory]
    [InlineData(48)]
    [InlineData(25)]
    [InlineData(720)]
    public void Cancel_MoreThan24HoursBeforeDeparture_ShouldSetStatusToCancelled(int hoursBeforeDeparture)
    {
        var currentUtc = DateTime.UtcNow;
        var flight = CreateFlight(currentUtc.AddHours(hoursBeforeDeparture));
        var booking = flight.AddBooking(CreatePassenger(), "1A");

        flight.CancelBooking(booking, currentUtc);
        booking.Status.Should().Be(BookingStatus.Cancelled);
    }

    [Theory]
    [InlineData(24)]
    [InlineData(23)]
    [InlineData(10)]
    [InlineData(0)]
    [InlineData(-5)]
    public void Cancel_24HoursOrLessBeforeDeparture_ShouldThrowInvalidOperationException(int hoursBeforeDeparture)
    {
        var currentUtc = DateTime.UtcNow;
        var flight = CreateFlight(currentUtc.AddHours(hoursBeforeDeparture));
        var booking = flight.AddBooking(CreatePassenger(), "1A");

        Action act = () => flight.CancelBooking(booking, currentUtc);
        act.Should().Throw<InvalidOperationException>().WithMessage("*Cannot cancel booking less than 24 hours*");
    }

    [Fact]
    public void Cancel_AlreadyCancelledBooking_ShouldThrowException()
    {
        var currentUtc = DateTime.UtcNow;
        var flight = CreateFlight(currentUtc.AddDays(3));
        var booking = flight.AddBooking(CreatePassenger(), "1A");

        flight.CancelBooking(booking, currentUtc);
        Action act = () => flight.CancelBooking(booking, currentUtc);

        act.Should().Throw<InvalidOperationException>().WithMessage("*already cancelled*");
    }

    [Theory]
    [InlineData(2)]
    [InlineData(0)]
    public void Board_BeforeOrAtDeparture_ShouldSetStatusToBoarded(int hoursBeforeDeparture)
    {
        var currentUtc = DateTime.UtcNow;
        var flight = CreateFlight(currentUtc.AddHours(hoursBeforeDeparture));
        var booking = flight.AddBooking(CreatePassenger(), "1A");

        booking.Board(currentUtc, flight);
        booking.Status.Should().Be(BookingStatus.Boarded);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(24)]
    public void Board_AfterDepartureTime_ShouldThrowException(int hoursAfterDeparture)
    {
        var departure = DateTime.UtcNow;
        var flight = CreateFlight(departure);
        var booking = flight.AddBooking(CreatePassenger(), "1A");

        Action act = () => booking.Board(departure.AddHours(hoursAfterDeparture), flight);
        act.Should().Throw<InvalidOperationException>().WithMessage("*Cannot board after flight departure time*");
    } 

    [Fact]
    public void Board_CancelledBooking_ShouldThrowException()
    {
        var currentUtc = DateTime.UtcNow;
        var flight = CreateFlight(currentUtc.AddDays(3));
        var booking = flight.AddBooking(CreatePassenger(), "1A");
        
        flight.CancelBooking(booking, currentUtc);
        Action act = () => booking.Board(currentUtc, flight);
        
        act.Should().Throw<InvalidOperationException>().WithMessage("*Only confirmed bookings can be boarded*");
    }
}