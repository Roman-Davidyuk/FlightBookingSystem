namespace Core.Entities;

public enum BookingStatus { Confirmed, Cancelled, Boarded }

public class Booking
{
    public Guid Id { get; private set; }
    public Guid FlightId { get; private set; }
    public Guid PassengerId { get; private set; }
    public string SeatNumber { get; private set; }
    public DateTime BookingDate { get; private set; }
    public BookingStatus Status { get; private set; }

    public Flight? Flight { get; private set; }
    public Passenger? Passenger { get; private set; }

    protected Booking() { }
    
    internal Booking(Guid flightId, Guid passengerId, string seatNumber)
    {
        if (string.IsNullOrWhiteSpace(seatNumber)) throw new ArgumentException("Seat number cannot be empty.");

        Id = Guid.NewGuid();
        FlightId = flightId;
        PassengerId = passengerId;
        SeatNumber = seatNumber;
        BookingDate = DateTime.UtcNow;
        Status = BookingStatus.Confirmed;
    }

    internal void Cancel(DateTime currentUtcTime, Flight flight)
    {
        if (Status == BookingStatus.Cancelled) throw new InvalidOperationException("Booking is already cancelled.");
        if (Status == BookingStatus.Boarded) throw new InvalidOperationException("Cannot cancel a boarded flight.");

        var timeUntilDeparture = flight.DepartureTime - currentUtcTime;
        if (timeUntilDeparture.TotalHours <= 24)
            throw new InvalidOperationException("Cannot cancel booking less than 24 hours before departure for a refund.");

        Status = BookingStatus.Cancelled;
    }

    public void Board(DateTime currentUtcTime, Flight flight)
    {
        if (Status != BookingStatus.Confirmed) throw new InvalidOperationException("Only confirmed bookings can be boarded.");
        
        if (currentUtcTime > flight.DepartureTime)
            throw new InvalidOperationException("Cannot board after flight departure time.");

        Status = BookingStatus.Boarded;
    }
}