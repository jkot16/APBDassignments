using Tutorial7Task.Contracts.Responses;

namespace Tutorial7Task.Services;

public interface IClientService
{
    Task<List<ClientTripResponse>> GetClientTripsAsync(int id, CancellationToken cancellationToken);
    
    Task<bool> ClientExistsAsync(int id, CancellationToken cancellationToken);
}