using Bogus;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Passengers.AnyAsync()) return;
        
        var passengerFaker = new Faker<Passenger>("en")
            .CustomInstantiator(f => new Passenger(
                f.Name.FirstName(),
                f.Name.LastName(),
                f.Internet.Email(),
                f.Random.String2(2, "ABCDEFGHIJKLMNOPQRSTUVWXYZ") + f.Random.String2(6, "0123456789")
            ));
        
        var passengers = passengerFaker.Generate(1000);
        await context.Passengers.AddRangeAsync(passengers);
        await context.SaveChangesAsync();
        
        var cities = new[] { "Kyiv", "Warsaw", "London", "New York", "Paris", "Berlin", "Dubai", "Tokyo", "Rome" };
        
        var flightFaker = new Faker<Flight>("en")
            .CustomInstantiator(f => {
                var departure = f.Date.Soon(30).ToUniversalTime();
                return new Flight(
                    f.Random.Replace("FL-###"),
                    f.PickRandom(cities),
                    f.PickRandom(cities), 
                    departure,
                    departure.AddHours(f.Random.Int(2, 12)),
                    f.Random.Int(150, 300),
                    Math.Round(f.Random.Decimal(50, 1000), 2)
                );
            });

        var flights = flightFaker.Generate(100);
        await context.Flights.AddRangeAsync(flights);
        await context.SaveChangesAsync();
        
        var random = new Random();

        foreach (var flight in flights)
        {
            for (int i = 1; i <= 89; i++)
            {
                var passenger = passengers[random.Next(passengers.Count)];
                
                var seatNumber = $"{i}{(char)random.Next('A', 'G')}"; 

                try
                {
                    flight.AddBooking(passenger, seatNumber);
                }
                catch
                {
                    // ignored
                }
            }
        }
        

        await context.SaveChangesAsync();
    }
}