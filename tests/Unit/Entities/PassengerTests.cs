using Core.Entities;
using FluentAssertions;
using Xunit;

namespace Unit.Entities;

public class PassengerTests
{
    [Fact]
    public void CreatePassenger_ValidData_ShouldInitializeCorrectly()
    {
        var passenger = new Passenger("John", "Doe", "john@example.com", "AB123456");
        
        passenger.FirstName.Should().Be("John");
        passenger.LastName.Should().Be("Doe");
        passenger.Email.Should().Be("john@example.com");
        passenger.PassportNumber.Should().Be("AB123456");
    }

    [Theory]
    [InlineData(null, "Doe")]
    [InlineData("", "Doe")]
    [InlineData("   ", "Doe")]
    [InlineData("John", null)]
    [InlineData("John", "")]
    public void CreatePassenger_InvalidName_ShouldThrowArgumentException(string firstName, string lastName)
    {
        Action act = () => new Passenger(firstName, lastName, "j@j.com", "AB123456");
        act.Should().Throw<ArgumentException>();
    } 

    [Theory]
    [InlineData("123456")]
    [InlineData("ABCD123")]
    [InlineData("AB12345")]
    [InlineData("ab123456")] 
    public void CreatePassenger_InvalidPassportFormat_ShouldThrowArgumentException(string invalidPassport)
    {
        Action act = () => new Passenger("John", "Doe", "j@j.com", invalidPassport);
        
        act.Should().Throw<ArgumentException>();
    } 
}