namespace ClinicManager.DTOs;

public class ProcedureDto
{
	public Guid Id { get; set; }
	public string Description { get; set; } = null!;
	public decimal ServiceCost { get; set; }
}