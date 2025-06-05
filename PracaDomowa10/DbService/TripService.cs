using Microsoft.EntityFrameworkCore;
using PracaDomowa10.Data;
using PracaDomowa10.DTOs;

namespace PracaDomowa10.DbService;

public interface ITripService
{
    Task<PaginatedTripDTO> GetSortedTrips(int pageNum, int pageSize);
}

public class TripService : ITripService
{
    private readonly IDbRepository _dbRepository;
    private readonly TripsDbContext _dbContext;

    public TripService(IDbRepository dbRepository, TripsDbContext dbContext)
    {
        _dbRepository = dbRepository;
        _dbContext = dbContext;
    }

    public async Task<PaginatedTripDTO> GetSortedTrips(int page = 1, int pageSize = 10)
    {
        var totalCount = await _dbContext.Trips.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var trips = await _dbContext.Trips
            .OrderByDescending(t => t.DateFrom)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TripDTO
            {
                Name = t.Name,
                Description = t.Description,
                Datefrom = t.DateFrom,
                DateTo = t.DateTo,
                MaxPeople = t.MaxPeople,
                Countries = t.IdCountries
                    .Select(c => new CountryDTO
                    {
                        Name = c.Name
                    }).ToList(),
                Clients = t.ClientTrips
                    .Select(ct => new ClientDTO
                    {
                        FirstName = ct.IdClientNavigation.FirstName,
                        LastName = ct.IdClientNavigation.LastName,
                    }).ToList()
            })
            .ToListAsync();

        return new PaginatedTripDTO
        {
            PageNum = page,
            PageSize = pageSize,
            AllPages = totalPages,
            Trips = trips
        };
    }
}