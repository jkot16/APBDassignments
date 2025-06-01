using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Tutorial9Task.Contracts.Requests;
using Tutorial9Task.Contracts.Responses;
using Tutorial9Task.Data;
using Tutorial9Task.Models;

namespace Tutorial9Task.Services;

public class TripService : ITripService {
    private readonly TravelDbContext _context;

    public TripService(TravelDbContext context) {
        _context = context;
    }

    public async Task<PagedTripResponse> GetTripsAsync(int page, int pageSize, CancellationToken cancellationToken) {
        var query = _context.Trips
            .Include(t => t.Client_Trips).ThenInclude(ct => ct.IdClientNavigation)
            .Include(t => t.IdCountries)
            .OrderByDescending(t => t.DateFrom);

        var total = await query.CountAsync();
        var trips = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PagedTripResponse {
            PageNum = page,
            PageSize = pageSize,
            AllPages = (int)Math.Ceiling(total / (double)pageSize),
            Trips = trips.Select(t => new TripDto {
                Name =t.Name,
                Description = t.Description,
                DateFrom = t.DateFrom,
                DateTo = t.DateTo,
                MaxPeople = t.MaxPeople,
                Countries = t.IdCountries.Select(c => c.Name).ToList(),
                Clients = t.Client_Trips.Select(ct => new ClientDto {
                    FirstName = ct.IdClientNavigation.FirstName,
                    LastName = ct.IdClientNavigation.LastName
                }).ToList()
            }).ToList()
        };
    }

    public async Task AssignClientToTripAsync(int tripId, AssignClientRequest request, CancellationToken cancellationToken) {
        var trip = await _context.Trips.FindAsync(tripId);
        if (trip == null || trip.DateFrom <= DateTime.Now)
            throw new Exception("Trip not found.");

        var existingClient = await _context.Clients.FirstOrDefaultAsync(c => c.Pesel == request.Pesel);
        if (existingClient != null)
            throw new Exception("Client with given PESEL already exists.");

        var alreadyAssigned = await _context.Client_Trips.AnyAsync(ct => ct.IdTrip == tripId && ct.IdClientNavigation.Pesel == request.Pesel);
        if (alreadyAssigned)
            throw new Exception("Client already registered for this trip.");

        var client = new Client {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Telephone = request.Telephone,
            Pesel = request.Pesel
        };

        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        var clientTrip = new Client_Trip {
            IdClient = client.IdClient,
            IdTrip= tripId,
            RegisteredAt = DateTime.UtcNow,
            PaymentDate = request.PaymentDate
        };

        _context.Client_Trips.Add(clientTrip);
        await _context.SaveChangesAsync();
    }
}
