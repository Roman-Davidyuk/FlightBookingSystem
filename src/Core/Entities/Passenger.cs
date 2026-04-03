using System.Text.RegularExpressions;

namespace Core.Entities;

public class Passenger
{
    public Guid Id { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public string PassportNumber { get; private set; }

    private readonly List<Booking> _bookings = new();
    public IReadOnlyCollection<Booking> Bookings => _bookings.AsReadOnly();

    protected Passenger() { }

    public Passenger(string firstName, string lastName, string email, string passportNumber)
    {
        if (string.IsNullOrWhiteSpace(firstName)) throw new ArgumentException("First name is required.");
        if (string.IsNullOrWhiteSpace(lastName)) throw new ArgumentException("Last name is required.");
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@')) throw new ArgumentException("Valid email is required.");
        
        if (string.IsNullOrWhiteSpace(passportNumber) || !Regex.IsMatch(passportNumber, @"^[A-Z]{2}\d{6}$"))
        {
            throw new ArgumentException("Invalid passport format. Must be 2 uppercase letters and 6 digits.", nameof(passportNumber));
        }

        Id = Guid.NewGuid();
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PassportNumber = passportNumber.ToUpperInvariant(); 
    }
}