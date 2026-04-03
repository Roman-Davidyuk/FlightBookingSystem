using Bogus;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (!await context.Passengers.AnyAsync())
        {
            var passengerFaker = new Faker<Passenger>("en")
                .CustomInstantiator(f => new Passenger(
                    f.Name.FirstName(), f.Name.LastName(), f.Internet.Email(),
                    f.Random.String2(2, "ABCDEFGHIJKLMNOPQRSTUVWXYZ") + f.Random.String2(6, "0123456789")
                ));
            await context.Passengers.AddRangeAsync(passengerFaker.Generate(1000));
            await context.SaveChangesAsync();
        }
        
        if (!await context.Flights.AnyAsync())
        {
            var cities = new[] { "Kyiv", "Warsaw", "London", "New York", "Paris", "Berlin", "Dubai", "Tokyo", "Rome" };
            var flightFaker = new Faker<Flight>("en")
                .CustomInstantiator(f => {
                    var departure = f.Date.Soon(30).ToUniversalTime();
                    return new Flight(
                        f.Random.Replace("FL-###"), f.PickRandom(cities), f.PickRandom(cities), 
                        departure, departure.AddHours(f.Random.Int(2, 12)),
                        f.Random.Int(150, 300), Math.Round(f.Random.Decimal(50, 1000), 2)
                    );
                });
            await context.Flights.AddRangeAsync(flightFaker.Generate(100));
            await context.SaveChangesAsync();
        }
        
        if (!await context.Bookings.AnyAsync())
        {
            var dbPassengers = await context.Passengers.ToListAsync();
            var dbFlights = await context.Flights.ToListAsync();
            var random = new Random();
            var bookingsToAdd = new List<Booking>();

            foreach (var flight in dbFlights)
            {
                for (int i = 1; i <= 89; i++)
                {
                    var passenger = dbPassengers[random.Next(dbPassengers.Count)];
                    var seatNumber = $"{i}{(char)random.Next('A', 'G')}"; 

                    try
                    {
                        var booking = flight.AddBooking(passenger, seatNumber);
                        bookingsToAdd.Add(booking);
                    }
                    catch (Exception ex)
                    {
                        // ignored
                    }
                }
            }
            
            foreach (var booking in bookingsToAdd)
            {
                context.Entry(booking).State = EntityState.Added;
            }
            await context.SaveChangesAsync();
        }
    }
}