using Microsoft.AspNetCore.Mvc;
using Tutorial9Task.Services;

namespace Tutorial9Task.Controllers;

[ApiController]
[Route("api/clients")]
public class ClientsController : ControllerBase
{
    private readonly IClientService _clientService;

    public ClientsController(IClientService clientService)
    {
        _clientService = clientService;
    }

    [HttpDelete("{idClient}")]
    public async Task<IActionResult> DeleteClient(int idClient, CancellationToken ct)
    {
        try
        {
            var success = await _clientService.DeleteClientAsync(idClient, ct);
            if (!success)
                return NotFound(new { message = "Client not found." });

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message }); // 409
        }
    }
}