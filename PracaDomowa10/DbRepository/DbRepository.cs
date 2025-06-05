using Microsoft.EntityFrameworkCore;
using PracaDomowa10.Data;
using PracaDomowa10.Models;

namespace PracaDomowa10;

public interface IDbRepository
{
    Task<Trip> GetTripByIdAsync(int IdTrip, CancellationToken cancellationToken);
    Task<List<Trip>> GetTripsAsync();
    
    Task DeleteClientAsync(int IdClient, CancellationToken cancellationToken);
    
    Task<bool> ClientHasTripsAsync(int IdClient, CancellationToken cancellationToken);
    
    Task<bool> DoesClientExistAsync(string Pesel, CancellationToken cancellationToken);
    
    Task<bool> ClientAlreadyRegisteredAsync(int IdTrip, string Pesel, CancellationToken cancellationToken);
    
    Task<bool> DoesTripExistAsync(int IdTrip, CancellationToken cancellationToken);
    Task<bool> TripInTheFuture(int IdTrip, CancellationToken cancellationToken);
    
    Task AddClient(Client client, CancellationToken cancellationToken);

    Task AddClientToTripAsync(ClientTrip clientTrip, CancellationToken cancellationToken);

}

public class DbRepository : IDbRepository
{
    private readonly TripsDbContext _context;

    public DbRepository(TripsDbContext context)
    {
        _context = context;
    }
    
    public async Task<Trip> GetTripByIdAsync(int IdTrip, CancellationToken cancellationToken)
    {
        return await _context.Trips.FirstOrDefaultAsync(t => t.IdTrip == IdTrip, cancellationToken);
    }

    public Task<List<Trip>> GetTripsAsync()
    {
        var trips = _context.Trips
            .Include(t => t.ClientTrips)
            .Include(t => t.IdCountries);
        return trips.ToListAsync();
    }

    public async Task DeleteClientAsync(int IdClient, CancellationToken cancellationToken)
    {
        var client = await _context.Clients.FindAsync(IdClient, cancellationToken);
        if (client != null)
        {
            _context.Clients.Remove(client);
            await _context.SaveChangesAsync(cancellationToken);
        }
        else
        {
            throw new KeyNotFoundException("Client not found");
        }
    }

    public async Task<bool> ClientHasTripsAsync(int IdClient, CancellationToken cancellationToken)
    {
        var hasTrips = await _context.ClientTrips.AnyAsync(t => t.IdClient == IdClient);
        return hasTrips;
    }

    public async Task<bool> DoesClientExistAsync(string Pesel, CancellationToken cancellationToken)
    {
        return await _context.Clients.AnyAsync(t => t.Pesel == Pesel, cancellationToken);
    }

    public async Task<bool> ClientAlreadyRegisteredAsync(int IdTrip, string Pesel, CancellationToken cancellationToken)
    {
        return await _context.ClientTrips.AnyAsync(
            ct => ct.IdTrip == IdTrip && ct.IdClientNavigation.Pesel == Pesel, cancellationToken);
    }

    public async Task<bool> DoesTripExistAsync(int IdTrip, CancellationToken cancellationToken)
    {
        return await _context.Trips.AnyAsync(t => t.IdTrip == IdTrip, cancellationToken);
    }

    public async Task<bool> TripInTheFuture(int IdTrip, CancellationToken cancellationToken)
    {
        var dateFrom = await _context.Trips.Where(t => t.IdTrip == IdTrip)
            .Select(t => t.DateFrom).FirstOrDefaultAsync();
        if (dateFrom > DateTime.Now)
            return true;
        return false;
    }

    public Task AddClient(Client client, CancellationToken cancellationToken)
    { 
        _context.Clients.Add(client);
        return _context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task AddClientToTripAsync(ClientTrip clientTrip, CancellationToken cancellationToken)
    {
        await _context.ClientTrips.AddAsync(clientTrip, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

}