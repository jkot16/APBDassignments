using System.Data;
using Microsoft.Data.SqlClient;
using Tutorial7Task.Contracts.Requests;
using Tutorial7Task.Contracts.Responses;


namespace Tutorial7Task.Services;


public class ClientService : IClientService
{
    private readonly string _cs;
    public ClientService(IConfiguration cfg)
        => _cs = cfg.GetConnectionString("DefaultConnection")!;

    public async Task<bool> ClientExistsAsync(int id, CancellationToken cancellationToken)
    {
        
        // SELECT 1 to check if given client exists
        const string sql = @"
            SELECT 1
            FROM Client
            WHERE IdClient = @id;
        ";

        await using var conn = new SqlConnection(_cs);
        await conn.OpenAsync(cancellationToken);
        await using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);  
        var result = await cmd.ExecuteScalarAsync(cancellationToken);
        return result != null;
    }

    public async Task<List<ClientTripResponse>> GetClientTripsAsync(int id, CancellationToken cancellationToken)
    {
        // 1) SELECT trips with chosen info
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
                    PaymentDate  = reader.IsDBNull(reader.GetOrdinal("PaymentDate")) ? null : (int)reader["PaymentDate"],
                });
            }
        }

        if (trips.Count == 0)
            return trips;

        // SELECT countries based on tripId
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

    public async Task<int> CreateClientAsync(CreateClientRequest request, CancellationToken cancellationToken)
    {
        
        // insert client info with chosen data (cast(scope_identity()) to add id automatically
        const string sqlInsertClient = @"
        INSERT INTO Client (FirstName, LastName, Email, Telephone, Pesel)
        VALUES (@FirstName, @LastName, @Email, @Telephone, @Pesel);
        SELECT CAST(SCOPE_IDENTITY() as int);
        ";
        
        await using var conn = new SqlConnection(_cs);
        await conn.OpenAsync(cancellationToken);
        
        var dbTrans = await conn.BeginTransactionAsync(cancellationToken);
        
        var tx = (SqlTransaction)dbTrans;

        try
        {
            await using var cmd = new SqlCommand(sqlInsertClient, conn, tx);
            cmd.Parameters.AddWithValue("@FirstName", request.FirstName);
            cmd.Parameters.AddWithValue("@LastName", request.LastName);
            cmd.Parameters.AddWithValue("@Email", request.Email);
            cmd.Parameters.AddWithValue("@Telephone", request.Telephone);
            cmd.Parameters.AddWithValue("@Pesel", request.Pesel);

            var idObj = await cmd.ExecuteScalarAsync(cancellationToken);
            int newId = Convert.ToInt32(idObj);

            await tx.CommitAsync(cancellationToken);
            return newId;
        }
        catch
        {
            await tx.RollbackAsync(cancellationToken);
            throw;
        }
        finally
        {
            tx.Dispose();
        }
    }

    public async Task<bool> TripExistsAsync(int tripId, CancellationToken cancellationToken)
    {
        
        // SELECT 1 to check if given trip exists
        const string sql = @"
        SELECT 1
        FROM Trip
        WHERE IdTrip = @tripId;
        ";
        
        await using var conn = new SqlConnection(_cs);
        await conn.OpenAsync(cancellationToken);
        await using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@tripId", tripId);  
        var result = await cmd.ExecuteScalarAsync(cancellationToken);
        return result != null;
        
    }

    public async Task<bool> IsTripFullAsync(int tripId, CancellationToken cancellationToken)
    {
        
        // SELECT Max number of people for given trip
        const string sqlTripMaxPeople = @"
        SELECT MaxPeople
        FROM Trip
        WHERE IdTrip = @tripId;
        ";

        
        // select actual count of people that will be attending this given trip
        const string sqlActualCount = @"
        SELECT COUNT(*)
        FROM Client_Trip
        WHERE IdTrip = @tripId;
        ";
        
        await using var conn = new SqlConnection(_cs);
        await conn.OpenAsync(cancellationToken);

        int maxPeople;

        await using (var cmd = new SqlCommand(sqlTripMaxPeople, conn))
        {
            cmd.Parameters.AddWithValue("@tripId", tripId);
            var maxPeopleObj = await cmd.ExecuteScalarAsync(cancellationToken);
            maxPeople = (int)maxPeopleObj;
        }

        await using (var cmd1 = new SqlCommand(sqlActualCount, conn))
        {
            cmd1.Parameters.AddWithValue("@tripId", tripId);
            var maxPeopleObj = await cmd1.ExecuteScalarAsync(cancellationToken);
            var currentMaxPeople = (int)maxPeopleObj;
            return currentMaxPeople >= maxPeople;
        }
    }

    public async Task RegisterClientTripAsync(int clientId, int tripId, CancellationToken cancellationToken)
    {
        
        
        // INSERT given info into Client_Trip table
        const string sqlInsertClientTrip = @"
        INSERT INTO Client_Trip(IdClient, IdTrip, RegisteredAt,PaymentDate)
        VALUES (@IdClient, @IdTrip, @RegisteredAt, NULL);
        ";
        
        var registeredAt = int.Parse(DateTime.Now.ToString("yyyyMMdd"));

        
        await using var conn = new SqlConnection(_cs);
        await conn.OpenAsync(cancellationToken);
        await using var tx = await conn.BeginTransactionAsync(cancellationToken) as SqlTransaction;
        
        await using var cmd = new SqlCommand(sqlInsertClientTrip, conn, tx);
        cmd.Parameters.AddWithValue("@IdClient", clientId);
        cmd.Parameters.AddWithValue("@IdTrip", tripId);
        cmd.Parameters.AddWithValue("@RegisteredAt", registeredAt);
        await cmd.ExecuteNonQueryAsync(cancellationToken);
        await tx.CommitAsync(cancellationToken);
        
    }

    public async Task<bool> RegistrationExistsAsync(int clientId, int tripId, CancellationToken cancellationToken)
    {
        
        // SELECT 1 to check if given registration for clientId and IdTrip exists
        const string sql = @"
        SELECT 1
        FROM Client_Trip
        WHERE IdClient = @clientId AND IdTrip   = @tripId;
    ";

        await using var conn = new SqlConnection(_cs);
        await conn.OpenAsync(cancellationToken);
        await using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@clientId", clientId);
        cmd.Parameters.AddWithValue("@tripId", tripId);
        var result = await cmd.ExecuteScalarAsync(cancellationToken);
        return result != null;
    }

    public async Task UnregisterClientTripAsync(int clientId, int tripId, CancellationToken cancellationToken)
    {
        
        // DELETE data based on clientId and tripId
        const string sql = @"
        DELETE FROM Client_Trip
        WHERE IdClient = @clientId AND IdTrip   = @tripId;
    ";

        await using var conn = new SqlConnection(_cs);
        await conn.OpenAsync(cancellationToken);
        await using var tx  = await conn.BeginTransactionAsync(cancellationToken) as SqlTransaction;
        await using var cmd = new SqlCommand(sql, conn, tx);
        cmd.Parameters.AddWithValue("@clientId",clientId);
        cmd.Parameters.AddWithValue("@tripId", tripId);
        await cmd.ExecuteNonQueryAsync(cancellationToken);
        await tx.CommitAsync(cancellationToken);
    }

}
