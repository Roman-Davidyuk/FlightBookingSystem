namespace Application.DTOs;

public class CreateBookingDto
{
    public Guid FlightId { get; set; }
    public Guid PassengerId { get; set; }
    public string SeatNumber { get; set; } = string.Empty;
}