using System.ComponentModel.DataAnnotations;

namespace Tutorial7Task.Contracts.Requests;

public class CreateClientRequest
{
    [Required, StringLength(20)]
    public string FirstName { get; set; } = null!;
    
    [Required, StringLength(20)]
    public string LastName  { get; set; } = null!;
    
    [Required, EmailAddress, StringLength(50)]
    public string Email     { get; set; } = null!;
    
    [Required, Phone, StringLength(12)]
    public string Telephone { get; set; } = null!;

    [Required, RegularExpression(@"^\d{11}$")]
    public string Pesel { get; set; } = null!; 
}