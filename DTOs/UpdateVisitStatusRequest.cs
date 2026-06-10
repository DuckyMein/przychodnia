using System.ComponentModel.DataAnnotations;

namespace ClinicManager.DTOs;

/// <summary>
/// DTO do zmiany statusu wizyty.
/// </summary>
public class UpdateVisitStatusRequest
{
    [Required(ErrorMessage = "Status jest wymagany")]
    [RegularExpression("^(Zaplanowana|WTrakcie|Zakonczona|Anulowana)$",
        ErrorMessage = "Status musi być jednym z: Zaplanowana, WTrakcie, Zakonczona, Anulowana")]
    public string Status { get; set; } = null!;
}
