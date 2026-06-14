namespace ClinicManager.DTOs;

public class PrescriptionDto
{
    public Guid Id { get; set; }
    public string AccessCode { get; set; } = null!; // 4-cyfrowy PIN
    public DateTime IssueDate { get; set; }
    public DateTime ExpiryDate { get; set; }

    // Zamiast całego obiektu pacjenta, przesyłamy tylko to, co chcemy wyświetlić
    public string PatientName { get; set; } = null!;

    public List<PrescriptionItemDto> Items { get; set; } = new();
}

public class PrescriptionItemDto
{
    public string MedicationName { get; set; } = null!; // np. Apap
    public string Dosage { get; set; } = null!;         // np. 2x dziennie
    public int Quantity { get; set; }                   // np. 1 opakowanie
}