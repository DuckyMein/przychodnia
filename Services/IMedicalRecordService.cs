using ClinicManager.DTOs;

namespace ClinicManager.Services;

/// <summary>
/// Interfejs serwisu kartoteki medycznej z audytem RODO.
/// </summary>
public interface IMedicalRecordService
{
    Task<List<MedicalRecordDto>> GetByPatientAsync(Guid patientId);
    Task<MedicalRecordDto?> GetByIdAsync(Guid id);
    Task<MedicalRecordDto> CreateAsync(CreateMedicalRecordRequest request, string? fileUrl);
    Task<MedicalRecordDto> UpdateAsync(Guid id, CreateMedicalRecordRequest request);
    Task DeleteAsync(Guid id);
}
