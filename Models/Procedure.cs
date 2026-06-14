namespace ClinicManager.Models;

public class Procedure
{
	public Guid Id { get; set; }
	public string Description { get; set; } = null!;
	public decimal ServiceCost { get; set; }
}