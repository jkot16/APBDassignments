using System.ComponentModel.DataAnnotations;

namespace Tutorial11Task.Contracts.Requests;

public class RegisterRequest
{
    [Required, MinLength(3, ErrorMessage = "Username has to have at least 3 characters")]
    public string Username { get; set; }
    
    [Required, MinLength(6, ErrorMessage = "Password has to have at least 6 characters")]
    public string Password { get; set; }
}