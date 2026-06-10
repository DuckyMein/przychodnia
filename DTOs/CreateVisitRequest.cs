using System.ComponentModel.DataAnnotations;

namespace ClinicManager.DTOs;

/// <summary>
/// DTO do tworzenia / aktualizacji wizyty.
/// </summary>
public class CreateVisitRequest
{
    [Required(ErrorMessage = "Pacjent jest wymagany")]
    public Guid PatientId { get; set; }

    [Required(ErrorMessage = "Lekarz jest wymagany")]
    public string AssignedDoctorId { get; set; } = null!;

    [Required(ErrorMessage = "Data wizyty jest wymagana")]
    public DateTime ScheduledAt { get; set; }

    [StringLength(1000, ErrorMessage = "Notatki nie mogą przekraczać 1000 znaków")]
    public string? Notes { get; set; }
}
