namespace ClinicManager.DTOs;

public class MedicationDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;        // Nazwa (np. Apap)
    public string ActiveIngredient { get; set; } = null!; // Składnik aktywny (np. Paracetamol)
    public string Form { get; set; } = null!;        // Forma (np. Tabletki)
    public string Strength { get; set; } = null!;    // Dawka (np. 500 mg)
}