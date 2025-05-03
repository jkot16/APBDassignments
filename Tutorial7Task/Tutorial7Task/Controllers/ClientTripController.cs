using Microsoft.AspNetCore.Mvc;
using Tutorial7Task.Services;
using Tutorial7Task.Contracts.Responses;
namespace Tutorial7Task.Controllers;



[ApiController]
[Route("api/clients/{id}/trips")]
public class ClientTripController : ControllerBase
{
    private readonly IClientService _clientService;
    public ClientTripController(IClientService clientService)
        => _clientService = clientService;

    [HttpGet]
    public async Task<ActionResult<List<ClientTripResponse>>> GetAllClientTrips(int id, CancellationToken cancellationToken)
    {
       
        if (!await _clientService.ClientExistsAsync(id, cancellationToken))
            return NotFound($"Client {id} does not exist");

     
        var trips = await _clientService.GetClientTripsAsync(id, cancellationToken);
        return Ok(trips);
    }
}
