using Microsoft.AspNetCore.Mvc;
using Tutorial10Task.Services;
using Tutorial10Task.Contracts.Requests;
namespace Tutorial10Task.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PrescriptionsController : ControllerBase
{
    private readonly IPrescriptionService _service;

    public PrescriptionsController(IPrescriptionService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> AddPrescription([FromBody] PrescriptionRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var newId = await _service.AddPrescriptionAsync(request, ct);
            return CreatedAtAction(nameof(AddPrescription), new { id = newId }, null);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("/api/patients/{id}")]
    public async Task<IActionResult> GetPatientDetails(int id, CancellationToken ct)
    {
        try
        {
            var response = await _service.GetPatientDetailsAsync(id, ct);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

}