using Microsoft.AspNetCore.Mvc;
using Tutorial7Task.Services;

namespace Tutorial7Task.Controllers;

[ApiController]
[Route("api/trips")]
public class TripsController : ControllerBase
{
  private readonly ITripService _tripService;

  public TripsController(ITripService tripService)
  {
    _tripService = tripService;
  }
  // simply get all trips that exist
  [HttpGet]
  public async Task<IActionResult> GetTrips(CancellationToken cancellationToken)
  {
    var dto = await _tripService.GetAllTripsAsync(cancellationToken);

    return Ok(dto);
    
  }
}