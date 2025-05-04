using Tutorial7Task.Services;
using Tutorial7Task.Contracts.Responses;
namespace Tutorial7Task.Services;

public interface ITripService
{
   
    
    Task<List<GetAllTripsResponse>> GetAllTripsAsync(CancellationToken cancellationToken);
}