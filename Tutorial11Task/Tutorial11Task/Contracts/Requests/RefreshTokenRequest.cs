using System.ComponentModel.DataAnnotations;

namespace Tutorial11Task.Contracts.Requests;

public class RefreshTokenRequest
{
    
    [Required(ErrorMessage = "Refresh token is rquired.")]
    public string RefreshToken { get; set; }
}