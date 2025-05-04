using Tutorial7Task.Contracts.Requests;
using Tutorial7Task.Contracts.Responses;

namespace Tutorial7Task.Services;

public interface IClientService
{
    // GET /api/clients/{id}/trips
    Task<List<ClientTripResponse>> GetClientTripsAsync(int id, CancellationToken cancellationToken);
    Task<bool> ClientExistsAsync(int id, CancellationToken cancellationToken); 

    // POST /api/clients
    Task<int> CreateClientAsync(CreateClientRequest request, CancellationToken cancellationToken);

    // PUT /api/clients/{id}/trips/{tripId}
    Task<bool> TripExistsAsync(int id, CancellationToken cancellationToken); 
    Task<bool> IsTripFullAsync(int id, CancellationToken cancellationToken); 
    Task RegisterClientTripAsync(int clientId, int tripId, CancellationToken cancellationToken);

    // DELETE /api/clients/{id}/trips/{tripId}
    Task<bool> RegistrationExistsAsync(int clientId, int tripId, CancellationToken cancellationToken); 
    Task UnregisterClientTripAsync(int clientId, int tripId, CancellationToken cancellationToken);
}