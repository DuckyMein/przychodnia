using System.ComponentModel.DataAnnotations;

namespace ClinicManager.DTOs;

public class CreateProcedureRequest
{
	[Required(ErrorMessage = "Opis procedury jest wymagany")]
	[StringLength(250, ErrorMessage = "Opis nie może przekraczać 250 znaków")]
	public string Description { get; set; } = null!;

	[Required(ErrorMessage = "Koszt świadczenia jest wymagany")]
	[Range(0.01, 100000.00, ErrorMessage = "Koszt musi być większy niż 0")]
	public decimal ServiceCost { get; set; }
}