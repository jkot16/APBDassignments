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

    
    // Get all trips related to client with specific id
    [HttpGet("{id}/trips")]
    public async Task<ActionResult<List<ClientTripResponse>>> GetAllClientTrips(int id, CancellationToken cancellationToken)
    {
       
        if (!await _clientService.ClientExistsAsync(id, cancellationToken))
            return NotFound($"Client {id} does not exist");

     
        var trips = await _clientService.GetClientTripsAsync(id, cancellationToken);
        return Ok(trips);
    }

    
    //Create new client record
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
    // Register specific client (by id) for a specific trip (tripId)

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
    
    // delete a client (id) registration from trip with given tripId
    [HttpDelete("{id}/trips/{tripId}")]
    public async Task<IActionResult> UnregisterClientFromTrip(int id, int tripId, CancellationToken cancellationToken)
    {
        if (!await _clientService.ClientExistsAsync(id, cancellationToken))
        {
            return NotFound($"Client {id} does not exist");
        }

        if (!await _clientService.TripExistsAsync(tripId, cancellationToken))
        {
            return NotFound($"Trip {tripId} does not exist");
        }

        if (!await _clientService.RegistrationExistsAsync(id, tripId, cancellationToken))
        {
            return NotFound($"Registration for client {id} on trip {tripId} does not exist");
        }
        await _clientService.UnregisterClientTripAsync(id, tripId, cancellationToken);
        
        return NoContent();
    }
}
