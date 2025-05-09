using Microsoft.AspNetCore.Mvc;
using Tutorial8Task.Contracts.Requests;
using Tutorial8Task.Contracts.Responses;
using Tutorial8Task.Services;


namespace Tutorial8Task.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WarehouseController : ControllerBase
    {
        private readonly IWarehouseService _service;
        public WarehouseController(IWarehouseService service)
            => _service = service;

        [HttpPost]
        public async Task<ActionResult<AddToWarehouseResponse>> AddAsync(
            [FromBody] AddToWarehouseRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var response = await _service.AddAsync(request, cancellationToken);
                return StatusCode(201, response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPost("with-procedure")]
        public async Task<ActionResult<AddToWarehouseResponse>> AddWithProcedureAsync(
            [FromBody] AddToWarehouseRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var response = await _service.AddWithProcedureAsync(request, cancellationToken);
                return StatusCode(201, response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }
    }
}