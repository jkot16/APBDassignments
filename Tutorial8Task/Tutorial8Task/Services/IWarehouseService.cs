using Tutorial8Task.Contracts.Requests;
using Tutorial8Task.Contracts.Responses;

namespace Tutorial8Task.Services;

public interface IWarehouseService
{
    Task<AddToWarehouseResponse> AddAsync(AddToWarehouseRequest request, CancellationToken cancellationToken);
    Task<AddToWarehouseResponse> AddWithProcedureAsync(AddToWarehouseRequest request, CancellationToken cancellationToken);
}