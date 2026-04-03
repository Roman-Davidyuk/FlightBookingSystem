namespace Core.Entities;

public class Flight
{
    public Guid Id { get; private set; }
    public string FlightNumber { get; private set; }
    public string Origin { get; private set; }
    public string Destination { get; private set; }
    public DateTime DepartureTime { get; private set; }
    public DateTime ArrivalTime { get; private set; }
    public int TotalSeats { get; private set; }
    public int AvailableSeats { get; private set; }
    public decimal Price { get; private set; }

    private readonly List<Booking> _bookings = new();
    public IReadOnlyCollection<Booking> Bookings => _bookings.AsReadOnly();

    protected Flight() { }

    public Flight(string flightNumber, string origin, string destination, DateTime departureTime, DateTime arrivalTime, int totalSeats, decimal price)
    {
        if (string.IsNullOrWhiteSpace(flightNumber)) throw new ArgumentException("Flight number is required.");
        if (string.IsNullOrWhiteSpace(origin) || string.IsNullOrWhiteSpace(destination)) throw new ArgumentException("Origin and destination are required.");
        if (departureTime >= arrivalTime) throw new ArgumentException("Departure time must be strictly before arrival time.");
        if (totalSeats <= 0) throw new ArgumentException("Total seats must be greater than zero.");
        if (price < 0) throw new ArgumentException("Price cannot be negative.");

        Id = Guid.NewGuid();
        FlightNumber = flightNumber;
        Origin = origin;
        Destination = destination;
        DepartureTime = departureTime;
        ArrivalTime = arrivalTime;
        TotalSeats = totalSeats;
        AvailableSeats = totalSeats; 
        Price = price;
    }
    
    public Booking AddBooking(Passenger passenger, string seatNumber)
    {
        if (passenger == null) throw new ArgumentNullException(nameof(passenger));

        if (string.IsNullOrWhiteSpace(seatNumber)) 
            throw new ArgumentException("Seat number cannot be empty.");
            
        if (_bookings.Any(b => b.SeatNumber == seatNumber && b.Status != BookingStatus.Cancelled))
            throw new InvalidOperationException($"Seat {seatNumber} is already booked.");

        if (AvailableSeats <= 0)
            throw new InvalidOperationException("No available seats on this flight.");

        AvailableSeats--;
        
        var booking = new Booking(this.Id, passenger.Id, seatNumber);
        _bookings.Add(booking);
        
        return booking;
    }

    public void CancelBooking(Booking booking, DateTime currentUtcTime)
    {
        booking.Cancel(currentUtcTime, this);
        
        if (AvailableSeats < TotalSeats)
        {
            AvailableSeats++;
        }
    }
}