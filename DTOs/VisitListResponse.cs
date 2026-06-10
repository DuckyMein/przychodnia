namespace ClinicManager.DTOs;

/// <summary>
/// DTO dla listy wizyt (widok uproszczony).
/// </summary>
public class VisitListResponse
{
    public Guid Id { get; set; }
    public string PatientName { get; set; } = null!;
    public string DoctorName { get; set; } = null!;
    public DateTime ScheduledAt { get; set; }
    public string Status { get; set; } = null!;
}
