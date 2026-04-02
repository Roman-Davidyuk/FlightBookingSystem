namespace Application.DTOs;

public class BookingDto
{
    public Guid Id { get; set; }
    public Guid FlightId { get; set; }
    public Guid PassengerId { get; set; }
    public string SeatNumber { get; set; } = string.Empty;
    public DateTime BookingDate { get; set; }
    public string Status { get; set; } = string.Empty;
}