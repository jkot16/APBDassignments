using Tutorial7Task.Dtos.Read;

namespace Tutorial7Task.Services;

public interface ITripService
{
   
    
    Task<List<GetAllTripsResponse>> GetAllTripsAsync(CancellationToken cancellationToken);
}