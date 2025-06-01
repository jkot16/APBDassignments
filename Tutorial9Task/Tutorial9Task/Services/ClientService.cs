using Microsoft.EntityFrameworkCore;
using Tutorial9Task.Data;
using Tutorial9Task.Models;

namespace Tutorial9Task.Services;

public class ClientService : IClientService
{
    private readonly TravelDbContext _context;

    public ClientService(TravelDbContext context)
    {
        _context = context;
    }

    public async Task<bool> DeleteClientAsync(int idClient, CancellationToken cancellationToken)
    {
        var client = await _context.Clients
            .Include(c => c.Client_Trips)
            .FirstOrDefaultAsync(c => c.IdClient == idClient);

        if (client == null)
            return false;

        if (client.Client_Trips.Any())
            throw new InvalidOperationException("Cannot delete client assigned to trips.");

        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();
        return true;
    }
}