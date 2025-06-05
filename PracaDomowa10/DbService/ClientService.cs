using PracaDomowa10.DTOs;
using PracaDomowa10.Models;

namespace PracaDomowa10.DbService;

public interface IClientService
{
    Task DeleteClientAync(int IdClient, CancellationToken cancellationToken);
    Task AddClientToTripAsync(ClientTripDTO clientTrip, CancellationToken cancellationToken);
}


public class ClientService : IClientService
{
    private readonly IDbRepository _dbRepository;

    public ClientService(IDbRepository dbRepository)
    {
        _dbRepository = dbRepository;
    }

    public async Task DeleteClientAync(int IdClient, CancellationToken cancellationToken)
    {
        var hasTrips = await _dbRepository.ClientHasTripsAsync(IdClient, cancellationToken);
        if (hasTrips)
            throw new InvalidOperationException("Client has trips");
        await _dbRepository.DeleteClientAsync(IdClient, cancellationToken);
    }

    public async Task AddClientToTripAsync(ClientTripDTO clientTrip, CancellationToken cancellationToken)
    {
        //sprawdź czy klient isntieje
        var clientExists = await _dbRepository.DoesClientExistAsync(clientTrip.Pesel, cancellationToken);
        if (clientExists)
            throw new InvalidOperationException("Client already exists");

        var client = new Client
        {
            FirstName = clientTrip.FirstName,
            LastName = clientTrip.LastName,
            Email = clientTrip.Email,
            Telephone = clientTrip.Telephone,
            Pesel = clientTrip.Pesel
        };

        await _dbRepository.AddClient(client, cancellationToken);

        //sprawdź czy klient już jest zapisany
        var clientRegistered = await _dbRepository.ClientAlreadyRegisteredAsync(
            clientTrip.IdTrip, clientTrip.Pesel, cancellationToken);
        if (clientRegistered)
            throw new InvalidOperationException("Client is already registered on this trip");

        //sprawdź czy wycieczka istnieje i jest w przyszłości
        var tripExists = await _dbRepository.DoesTripExistAsync(clientTrip.IdTrip, cancellationToken);
        var tripInTheFuture = await _dbRepository.TripInTheFuture(clientTrip.IdTrip, cancellationToken);
        if (!tripExists || !tripInTheFuture)
            throw new InvalidOperationException("Trip does not exists or has already happened");

        var clientTripEntity = new ClientTrip
        {
            IdClient = client.IdClient,
            IdTrip = clientTrip.IdTrip,
            RegisteredAt = DateTime.UtcNow,
            PaymentDate = clientTrip.PaymentDate != default ? clientTrip.PaymentDate : null
        };

        await _dbRepository.AddClientToTripAsync(clientTripEntity, cancellationToken);

    }
}