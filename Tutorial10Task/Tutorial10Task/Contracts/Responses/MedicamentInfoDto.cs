namespace Tutorial10Task.Contracts.Responses;

public class MedicamentInfoDto
{
    public int IdMedicament { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int? Dose { get; set; }
}