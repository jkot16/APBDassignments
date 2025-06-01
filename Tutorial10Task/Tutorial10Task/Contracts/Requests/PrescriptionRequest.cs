using System.ComponentModel.DataAnnotations;

namespace Tutorial10Task.Contracts.Requests;

public class PrescriptionRequest
{
    [Required]
    public DateTime Date { get; set; }

    [Required]
    public DateTime DueDate { get; set; }

    [Required]
    public PatientDto Patient { get; set; }

    [Required]
    public int IdDoctor { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "At least one medicament must be provided")]
    [MaxLength(10, ErrorMessage = "No more than 10 medicaments allowed")]
    public List<MedicamentDto> Medicaments { get; set; }
}