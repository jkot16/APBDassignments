using System.ComponentModel.DataAnnotations;

namespace Tutorial10Task.Contracts.Requests;

public class MedicamentDto
{
    [Required]
    public int IdMedicament { get; set; }
    
    public int? Dose { get; set; }
    
    [Required]
    public string Details { get; set; }
}