using System.Data;
using Microsoft.Data.SqlClient;
using Tutorial7Task.Contracts.Responses;


namespace Tutorial7Task.Services;

public class TripService : ITripService
{
    private readonly string _cs;

    public TripService(IConfiguration cfg)
    {
        _cs = cfg.GetConnectionString("DefaultConnection")!;
    }

    

    public async Task<List<GetAllTripsResponse>> GetAllTripsAsync(CancellationToken cancellationToken)
{
    const string sqlTrips = @"
        SELECT 
        IdTrip,
        Name,
        Description,
        DateFrom,
        DateTo,
        MaxPeople
        FROM Trip;
    ";

    var trips = new List<GetAllTripsResponse>();

    await using var conn = new SqlConnection(_cs);
    await conn.OpenAsync(cancellationToken);

    // We download all trips
    await using (var cmdTrip = new SqlCommand(sqlTrips, conn))
    await using (var readerTrip = await cmdTrip.ExecuteReaderAsync(cancellationToken))
    {
        while (await readerTrip.ReadAsync(cancellationToken))
        {
            trips.Add(new GetAllTripsResponse
            {
                IdTrip = (int)readerTrip["IdTrip"],
                Name = (string)readerTrip["Name"],
                Description = (string)readerTrip["Description"],
                DateFrom  = (DateTime)readerTrip["DateFrom"],
                DateTo = (DateTime)readerTrip["DateTo"],
                MaxPeople = (int)readerTrip["MaxPeople"],
            });
        }
    }

   // We download all countries
    const string sqlCountries = @"
        SELECT 
        ct.IdCountry,
        c.Name
        FROM Country_Trip ct
        JOIN Country c ON ct.IdCountry = c.IdCountry
        WHERE ct.IdTrip = @TripId;
    ";

    foreach (var trip in trips)
    {
        await using var cmdCountry = new SqlCommand(sqlCountries, conn);
        cmdCountry.Parameters.AddWithValue("@TripId", trip.IdTrip);

        await using var readerCountry = await cmdCountry.ExecuteReaderAsync(cancellationToken);
        while (await readerCountry.ReadAsync(cancellationToken))
        {
            trip.Countries.Add(new CountryResponse
            {
                IdCountry = (int)readerCountry["IdCountry"],
                Name = (string)readerCountry["Name"]
            });
        }
    }
    return trips;

    }
}
