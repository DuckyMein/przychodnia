using ClinicManager.Models;

namespace ClinicManager.Data;

public class Prescription
{
    public Guid Id { get; set; }
    public string AccessCode { get; set; } = null!; // 4-cyfrowy kod e-recepty
    public DateTime IssueDate { get; set; } = DateTime.UtcNow;
    public DateTime ExpiryDate { get; set; } = DateTime.UtcNow.AddDays(30);

    // Powiązanie z Pacjentem
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    // Relacja: jedna recepta ma wiele pozycji (leków)
    public ICollection<PrescriptionItem> PrescriptionItems { get; set; } = new List<PrescriptionItem>();
}