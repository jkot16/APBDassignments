using Microsoft.AspNetCore.Mvc;
using Tutorial11Task.Contracts.Requests;
using Tutorial11Task.Services;

namespace Tutorial11Task.Controllers;

[ApiController]
[Route("api")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) => _authService = authService;

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken cancellationToken)
    {

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        
        
        await _authService.Register(request, cancellationToken);
        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var result = await _authService.Login(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var result = await _authService.RefreshToken(token, request.RefreshToken, cancellationToken);
        return Ok(result);
    }

}