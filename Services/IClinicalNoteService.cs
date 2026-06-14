using ClinicManager.DTOs;

namespace ClinicManager.Services;

public interface IClinicalNoteService
{
    // Będziemy pobierać notatki tylko dla konkretnego pacjenta
    Task<IEnumerable<ClinicalNoteDto>> GetByPatientIdAsync(Guid patientId);
    Task<Guid> CreateAsync(CreateClinicalNoteRequest request);
}
