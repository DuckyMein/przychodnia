namespace ClinicManager.Models;

/// <summary>
/// Dokumentacja medyczna pacjenta (kartoteka).
/// Każdy dostęp jest logowany w AuditLog (RODO).
/// </summary>
public class MedicalRecord
{
    public Guid Id { get; set; }

    // Pacjent, którego dotyczy dokumentacja
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    // URL do skanu / zdjęcia dokumentu (np. /uploads/skan123.pdf)
    public string? DocumentScanUrl { get; set; }

    // Opis dokumentu (np. "Skierowanie do specjalisty – ortopeda")
    public string Description { get; set; } = null!;

    // Data dodania
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}
