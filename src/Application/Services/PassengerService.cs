using Application.DTOs;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;

namespace Application.Services;

public class PassengerService : IPassengerService
{
    private readonly IPassengerRepository _passengerRepository;

    public PassengerService(IPassengerRepository passengerRepository)
    {
        _passengerRepository = passengerRepository;
    }

    public async Task<IEnumerable<PassengerDto>> GetAllPassengersAsync(CancellationToken ct)
    {
        var passengers = await _passengerRepository.GetAllAsync(ct);
        
        return passengers.Select(p => new PassengerDto
        {
            Id = p.Id,
            FirstName = p.FirstName,
            LastName = p.LastName,
            Email = p.Email,
            PassportNumber = p.PassportNumber
        });
    }
}