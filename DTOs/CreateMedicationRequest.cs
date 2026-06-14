namespace ClinicManager.DTOs;

public class CreateMedicationRequest
{
    public string Name { get; set; } = null!;
    public string ActiveIngredient { get; set; } = null!;
    public string Form { get; set; } = null!;
    public string Strength { get; set; } = null!;
}