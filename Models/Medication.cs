namespace ClinicManager.Data;

public class Medication
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;        // np. Amotaks
    public string ActiveIngredient { get; set; } = null!; // np. Amoxicillinum
    public string Form { get; set; } = null!;        // np. Tabletki, Syrop
    public string Strength { get; set; } = null!;    // np. 500 mg

    // Relacja: jeden lek może być na wielu receptach
    public ICollection<PrescriptionItem> PrescriptionItems { get; set; } = new List<PrescriptionItem>();
}