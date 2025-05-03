using System.Data;
using Microsoft.Data.SqlClient;
using Tutorial7Task.Contracts.Responses;


namespace Tutorial7Task.Services;


public class ClientService : IClientService
{
    private readonly string _cs;
    public ClientService(IConfiguration cfg)
        => _cs = cfg.GetConnectionString("DefaultConnection")!;

    public async Task<bool> ClientExistsAsync(int id, CancellationToken cancellationToken)
    {
        const string sql = @"
            SELECT 1
            FROM Client
            WHERE IdClient = @id;
        ";

        await using var conn = new SqlConnection(_cs);
        await conn.OpenAsync(cancellationToken);
        await using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);   // <-- tu parametr @id
        var result = await cmd.ExecuteScalarAsync(cancellationToken);
        return result != null;
    }

    public async Task<List<ClientTripResponse>> GetClientTripsAsync(int id, CancellationToken cancellationToken)
    {
        // 1) SELECT trips info
        const string sqlTrips = @"
            SELECT 
                ct.IdTrip,
                t.Name,
                t.Description,
                t.DateFrom,
                t.DateTo,
                t.MaxPeople,
                ct.RegisteredAt,
                ct.PaymentDate
            FROM Client_Trip ct
            JOIN Trip t ON ct.IdTrip = t.IdTrip
            WHERE ct.IdClient = @id
            ORDER BY ct.IdTrip;
        ";

        var trips = new List<ClientTripResponse>();
        await using var conn = new SqlConnection(_cs);
        await conn.OpenAsync(cancellationToken);

        await using (var cmdTrips = new SqlCommand(sqlTrips, conn))
        {
            cmdTrips.Parameters.AddWithValue("@id", id);
            await using var reader = await cmdTrips.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                trips.Add(new ClientTripResponse
                {
                    IdTrip       = (int)reader["IdTrip"],
                    Name         = (string)reader["Name"],
                    Description  = (string)reader["Description"],
                    DateFrom     = (DateTime)reader["DateFrom"],
                    DateTo       = (DateTime)reader["DateTo"],
                    MaxPeople    = (int)reader["MaxPeople"],
                    RegisteredAt = (int)reader["RegisteredAt"],
                    PaymentDate  = reader.IsDBNull(reader.GetOrdinal("PaymentDate"))
                                     ? null
                                     : (int)reader["PaymentDate"],
                });
            }
        }

        if (trips.Count == 0)
            return trips;

        // SELECT countries for based on tripId
        const string sqlCountries = @"
            SELECT 
             ct.IdTrip,
             ct.IdCountry,
             c.Name AS CountryName
            FROM Country_Trip ct
            JOIN Country c ON ct.IdCountry = c.IdCountry
            WHERE ct.IdTrip = @tripId;
        ";

        foreach (var trip in trips)
        {
            await using var cmdCountry = new SqlCommand(sqlCountries, conn);
            cmdCountry.Parameters.AddWithValue("@tripId", trip.IdTrip);
            await using var readerCountry = await cmdCountry.ExecuteReaderAsync(cancellationToken);
            while (await readerCountry.ReadAsync(cancellationToken))
            {
                trip.Countries.Add(new CountryResponse
                {
                    IdCountry = (int)readerCountry["IdCountry"],
                    Name      = (string)readerCountry["CountryName"]
                });
            }
        }

        return trips;
    }
}
