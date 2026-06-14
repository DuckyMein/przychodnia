using ClinicManager.DTOs;

namespace ClinicManager.Services;

public interface IPrescriptionService
{
    Task<IEnumerable<PrescriptionDto>> GetAllAsync();
    Task<Guid> CreateAsync(CreatePrescriptionRequest request);
}
