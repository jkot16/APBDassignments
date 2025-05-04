using Microsoft.AspNetCore.Mvc;
using Tutorial7Task.Contracts.Requests;
using Tutorial7Task.Services;
using Tutorial7Task.Contracts.Responses;
namespace Tutorial7Task.Controllers;



[ApiController]
[Route("api/clients")]
public class ClientTripController : ControllerBase
{
    private readonly IClientService _clientService;
    public ClientTripController(IClientService clientService)
        => _clientService = clientService;

    [HttpGet("{id}/trips")]
    public async Task<ActionResult<List<ClientTripResponse>>> GetAllClientTrips(int id, CancellationToken cancellationToken)
    {
       
        if (!await _clientService.ClientExistsAsync(id, cancellationToken))
            return NotFound($"Client {id} does not exist");

     
        var trips = await _clientService.GetClientTripsAsync(id, cancellationToken);
        return Ok(trips);
    }

    [HttpPost]
    public async Task<IActionResult> CreateClient([FromBody] CreateClientRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var newId = await _clientService.CreateClientAsync(request, cancellationToken);
        
        
        return CreatedAtAction(
            nameof(GetAllClientTrips),
            new { id = newId },
            new CreateClientResponse {IdClient = newId}
            );
    }

    [HttpPut("{id}/trips/{tripId}")]
    public async Task<IActionResult> RegisterClientToTrip(int id, int tripId, CancellationToken cancellationToken)
    {
        if (!await _clientService.ClientExistsAsync(id, cancellationToken))
        {
            return NotFound($"Client {id} does not exist");
        }

        if (!await _clientService.TripExistsAsync(tripId, cancellationToken))
        {
            return NotFound($"Trip {tripId} does not exist");
        }

        if (await _clientService.IsTripFullAsync(tripId, cancellationToken))
        {
            return Conflict($"Trip {tripId} has reached maximum participants");
        }
        
        await _clientService.RegisterClientTripAsync(id, tripId, cancellationToken);
        return NoContent();
    }
}
