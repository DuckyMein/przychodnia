namespace ClinicManager.DTOs;

/// <summary>
/// DTO zwracany przy GET – dane kartoteki z nazwą pacjenta.
/// </summary>
public class MedicalRecordDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = null!; // "FirstName LastName"
    public string? DocumentScanUrl { get; set; }
    public string Description { get; set; } = null!;
    public DateTime UploadedAt { get; set; }
}
