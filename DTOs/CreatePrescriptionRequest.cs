namespace ClinicManager.DTOs;

public class CreatePrescriptionRequest
{
    public Guid PatientId { get; set; } // Dla kogo recepta

    // Lista leków dodanych do tej konkretnej recepty
    public List<PrescriptionItemRequest> Items { get; set; } = new();
}

public class PrescriptionItemRequest
{
    public Guid MedicationId { get; set; } // Wybrany lek z katalogu
    public string Dosage { get; set; } = null!;
    public int Quantity { get; set; }
}