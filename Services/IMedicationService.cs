using ClinicManager.DTOs;

namespace ClinicManager.Services;

public interface IMedicationService
{
    Task<IEnumerable<MedicationDto>> GetAllAsync();
    Task<Guid> CreateAsync(CreateMedicationRequest request);
}