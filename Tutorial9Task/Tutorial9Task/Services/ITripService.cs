using Tutorial9Task.Contracts.Requests;
using Tutorial9Task.Contracts.Responses;

namespace Tutorial9Task.Services;

public interface ITripService {
    Task<PagedTripResponse> GetTripsAsync(int page, int pageSize, CancellationToken cancellationToken);
    Task AssignClientToTripAsync(int tripId, AssignClientRequest request, CancellationToken cancellationToken);
}