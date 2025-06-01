using Microsoft.AspNetCore.Mvc;
using Tutorial9Task.Contracts.Requests;
using Tutorial9Task.Services;

namespace Tutorial9Task.Controllers;

[ApiController]
[Route("api/trips")]
public class TripsController : ControllerBase {
    private readonly ITripService _service;

    public TripsController(ITripService service) {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetTrips([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default) {
        return Ok(await _service.GetTripsAsync(page, pageSize, ct));
    }

    [HttpPost("{idTrip}/clients")]
    public async Task<IActionResult> AssignClient(int idTrip, [FromBody] AssignClientRequest request, CancellationToken ct) {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        try {
            await _service.AssignClientToTripAsync(idTrip, request, ct);
            return CreatedAtAction(nameof(GetTrips), null);
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }
}