using ClinicManager.DTOs;

namespace ClinicManager.Services;

public interface IProcedureService
{
    Task<List<ProcedureDto>> GetAllAsync();
    Task<ProcedureDto?> GetByIdAsync(Guid id);
    Task<ProcedureDto> CreateAsync(CreateProcedureRequest request);
    Task<ProcedureDto> UpdateAsync(Guid id, CreateProcedureRequest request);
    Task DeleteAsync(Guid id);
}