namespace ClinicManager.Models;

/// <summary>
/// Wizyta pacjenta – powiązana z pacjentem i lekarzem.
/// </summary>
public class Visit
{
    public Guid Id { get; set; }

    // Pacjent
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    // Lekarz (IdentityUser)
    public string AssignedDoctorId { get; set; } = null!;
    public Microsoft.AspNetCore.Identity.IdentityUser AssignedDoctor { get; set; } = null!;

    // Data i godzina wizyty
    public DateTime ScheduledAt { get; set; }

    // Status
    public VisitStatus Status { get; set; } = VisitStatus.Zaplanowana;

    // Koszt całkowity (suma kosztów procedur + leków – aktualizowany po zakończeniu)
    public decimal TotalCost { get; set; }

    // Dodatkowe notatki do wizyty
    public string? Notes { get; set; }
}

/// <summary>
/// Statusy wizyty.
/// </summary>
public enum VisitStatus
{
    Zaplanowana = 0,
    WTrakcie = 1,
    Zakonczona = 2,
    Anulowana = 3
}
