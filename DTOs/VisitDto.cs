namespace ClinicManager.DTOs;

/// <summary>
/// DTO zwracany przy GET – pełne dane wizyty z nazwą pacjenta i lekarza.
/// </summary>
public class VisitDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = null!;
    public string AssignedDoctorId { get; set; } = null!;
    public string DoctorName { get; set; } = null!;
    public DateTime ScheduledAt { get; set; }
    public string Status { get; set; } = null!;
    public decimal TotalCost { get; set; }
    public string? Notes { get; set; }
}
