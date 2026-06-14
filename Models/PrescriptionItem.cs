namespace ClinicManager.Data;

public class PrescriptionItem
{
    public Guid Id { get; set; }

    // Klucz do recepty
    public Guid PrescriptionId { get; set; }
    public Prescription Prescription { get; set; } = null!;

    // Klucz do katalogu leków
    public Guid MedicationId { get; set; }
    public Medication Medication { get; set; } = null!;

    // Dane specyficzne dla tego jednego wydania leku
    public string Dosage { get; set; } = null!; // np. "1 tabletka rano, 1 wieczorem"
    public int Quantity { get; set; } = 1;      // np. 2 opakowania
}