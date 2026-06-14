namespace ClinicManager.DTOs;

public class ClinicalNoteDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public string PatientName { get; set; } = null!;
}

public class CreateClinicalNoteRequest
{
    public Guid PatientId { get; set; }
    public string Content { get; set; } = null!;
}