using ClinicManager.DTOs;

namespace ClinicManager.Services;

public interface IVisitService
{
    Task<List<VisitDto>> GetAllAsync();
    Task<List<VisitDto>> GetByPatientAsync(Guid patientId);
    Task<List<VisitDto>> GetByDoctorAsync(string doctorId);
    Task<List<VisitDto>> GetTodayAsync();          // SQL Profiler endpoint
    Task<List<VisitListResponse>> GetActiveAsync(); // NBomber endpoint (z JOIN-ami)
    Task<List<VisitDto>> GetUpcomingAsync(DateTime date); // BackgroundService
    Task<VisitDto?> GetByIdAsync(Guid id);
    Task<VisitDto> CreateAsync(CreateVisitRequest request);
    Task<VisitDto> UpdateAsync(Guid id, CreateVisitRequest request);
    Task<VisitDto> UpdateStatusAsync(Guid id, string status);
    Task CancelAsync(Guid id); // anulowanie wizyty (soft – ustawia status Anulowana)
}
