using Tutorial7Task.Contracts.Requests;
using Tutorial7Task.Contracts.Responses;

namespace Tutorial7Task.Services;

public interface IClientService
{
    Task<List<ClientTripResponse>> GetClientTripsAsync(int id, CancellationToken cancellationToken);
    
    Task<bool> ClientExistsAsync(int id, CancellationToken cancellationToken);
    
    Task<int> CreateClientAsync(CreateClientRequest request, CancellationToken cancellationToken);
    
    Task<bool> TripExistsAsync(int id, CancellationToken cancellationToken);
    
    Task<bool> IsTripFullAsync(int id, CancellationToken cancellationToken);
    
    Task RegisterClientTripAsync(int clientId, int tripId, CancellationToken cancellationToken);
    
    
    
    
}