using Tutorial11Task.Contracts.Requests;
using Tutorial11Task.Contracts.Responses;

namespace Tutorial11Task.Services;

public interface IAuthService
{
    Task Register(RegisterRequest request, CancellationToken cancellationToken);
    Task<AuthResponse> Login(LoginRequest request, CancellationToken cancellationToken);
    Task<AuthResponse> RefreshToken(string accessToken, string refreshToken, CancellationToken cancellationToken);

}