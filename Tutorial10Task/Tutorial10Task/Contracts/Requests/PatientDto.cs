using System.ComponentModel.DataAnnotations;

namespace Tutorial10Task.Contracts.Requests;

public class PatientDto
{
    [Required]
    public int IdPatient { get; set; }

    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    [Required]
    public DateTime Birthdate { get; set; }
}