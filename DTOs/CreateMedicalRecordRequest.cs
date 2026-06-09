using System.ComponentModel.DataAnnotations;

namespace ClinicManager.DTOs;

/// <summary>
/// DTO do tworzenia nowego wpisu w kartotece.
/// </summary>
public class CreateMedicalRecordRequest
{
    [Required(ErrorMessage = "ID pacjenta jest wymagane")]
    public Guid PatientId { get; set; }

    [Required(ErrorMessage = "Opis dokumentu jest wymagany")]
    [StringLength(500, ErrorMessage = "Opis nie może przekraczać 500 znaków")]
    public string Description { get; set; } = null!;

    // Plik jest wysyłany osobno przez IFormFile w kontrolerze
}
