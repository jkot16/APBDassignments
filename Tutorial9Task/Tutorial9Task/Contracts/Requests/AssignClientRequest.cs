using System.ComponentModel.DataAnnotations;

namespace Tutorial9Task.Contracts.Requests;

public class AssignClientRequest {
    
    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; }

    [Required, Phone]
    public string Telephone { get; set; }

    [Required]
    public string Pesel { get; set; }
    public DateTime? PaymentDate { get; set; }
    
}