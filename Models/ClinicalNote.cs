namespace ClinicManager.Models;

public class ClinicalNote
{
    public Guid Id { get; set; }

    // Klucz obcy do Pacjenta
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    // Treść notatki
    public string Content { get; set; } = null!;

    // Data utworzenia (ustawia się automatycznie)
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}