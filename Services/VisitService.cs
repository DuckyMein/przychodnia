using Microsoft.EntityFrameworkCore;
using ClinicManager.Data;
using ClinicManager.DTOs;
using ClinicManager.Mappers;
using ClinicManager.Models;

namespace ClinicManager.Services;

public class VisitService : IVisitService
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<VisitService> _logger;

    public VisitService(ApplicationDbContext db, ILogger<VisitService> logger)
    {
        _db = db;
        _logger = logger;
    }

    /// <summary>
    /// Wszystkie wizyty (z nazwą pacjenta i lekarza).
    /// </summary>
    public async Task<List<VisitDto>> GetAllAsync()
    {
        _logger.LogInformation("Pobieranie wszystkich wizyt");

        return await _db.Visits
            .Include(v => v.Patient)
            .Include(v => v.AssignedDoctor)
            .OrderByDescending(v => v.ScheduledAt)
            .Select(v => new VisitDto
            {
                Id = v.Id,
                PatientId = v.PatientId,
                PatientName = v.Patient.FirstName + " " + v.Patient.LastName,
                AssignedDoctorId = v.AssignedDoctorId,
                DoctorName = v.AssignedDoctor.Email!,
                ScheduledAt = v.ScheduledAt,
                Status = v.Status.ToString(),
                TotalCost = v.TotalCost,
                Notes = v.Notes
            })
            .ToListAsync();
    }

    /// <summary>
    /// Wizyty danego pacjenta.
    /// </summary>
    public async Task<List<VisitDto>> GetByPatientAsync(Guid patientId)
    {
        _logger.LogInformation("Pobieranie wizyt pacjenta {PatientId}", patientId);

        return await _db.Visits
            .Include(v => v.Patient)
            .Include(v => v.AssignedDoctor)
            .Where(v => v.PatientId == patientId)
            .OrderByDescending(v => v.ScheduledAt)
            .Select(v => new VisitDto
            {
                Id = v.Id,
                PatientId = v.PatientId,
                PatientName = v.Patient.FirstName + " " + v.Patient.LastName,
                AssignedDoctorId = v.AssignedDoctorId,
                DoctorName = v.AssignedDoctor.Email!,
                ScheduledAt = v.ScheduledAt,
                Status = v.Status.ToString(),
                TotalCost = v.TotalCost,
                Notes = v.Notes
            })
            .ToListAsync();
    }

    /// <summary>
    /// Wizyty przypisane do konkretnego lekarza.
    /// </summary>
    public async Task<List<VisitDto>> GetByDoctorAsync(string doctorId)
    {
        _logger.LogInformation("Pobieranie wizyt lekarza {DoctorId}", doctorId);

        return await _db.Visits
            .Include(v => v.Patient)
            .Include(v => v.AssignedDoctor)
            .Where(v => v.AssignedDoctorId == doctorId)
            .OrderByDescending(v => v.ScheduledAt)
            .Select(v => new VisitDto
            {
                Id = v.Id,
                PatientId = v.PatientId,
                PatientName = v.Patient.FirstName + " " + v.Patient.LastName,
                AssignedDoctorId = v.AssignedDoctorId,
                DoctorName = v.AssignedDoctor.Email!,
                ScheduledAt = v.ScheduledAt,
                Status = v.Status.ToString(),
                TotalCost = v.TotalCost,
                Notes = v.Notes
            })
            .ToListAsync();
    }

    /// <summary>
    /// Wizyty na dzisiaj – endpoint dla SQL Profiler.
    /// </summary>
    public async Task<List<VisitDto>> GetTodayAsync()
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        _logger.LogInformation("Pobieranie wizyt na dziś: {Date}", today);

        return await _db.Visits
            .Include(v => v.Patient)
            .Include(v => v.AssignedDoctor)
            .Where(v => v.ScheduledAt >= today && v.ScheduledAt < tomorrow)
            .OrderBy(v => v.ScheduledAt)
            .Select(v => new VisitDto
            {
                Id = v.Id,
                PatientId = v.PatientId,
                PatientName = v.Patient.FirstName + " " + v.Patient.LastName,
                AssignedDoctorId = v.AssignedDoctorId,
                DoctorName = v.AssignedDoctor.Email!,
                ScheduledAt = v.ScheduledAt,
                Status = v.Status.ToString(),
                TotalCost = v.TotalCost,
                Notes = v.Notes
            })
            .ToListAsync();
    }

    /// <summary>
    /// Aktywne wizyty (Zaplanowana + WTrakcie) – endpoint dla NBomber.
    /// Zawiera JOIN-y: Patient, AssignedDoctor.
    /// </summary>
    public async Task<List<VisitListResponse>> GetActiveAsync()
    {
        _logger.LogInformation("Pobieranie aktywnych wizyt");

        return await _db.Visits
            .Include(v => v.Patient)
            .Include(v => v.AssignedDoctor)
            .Where(v => v.Status == VisitStatus.Zaplanowana || v.Status == VisitStatus.WTrakcie)
            .OrderBy(v => v.ScheduledAt)
            .Take(100)
            .Select(v => new VisitListResponse
            {
                Id = v.Id,
                PatientName = v.Patient.FirstName + " " + v.Patient.LastName,
                DoctorName = v.AssignedDoctor.Email!,
                ScheduledAt = v.ScheduledAt,
                Status = v.Status.ToString()
            })
            .ToListAsync();
    }

    /// <summary>
    /// Wizyty zaplanowane na konkretny dzień – dla BackgroundService (raport e-mail).
    /// </summary>
    public async Task<List<VisitDto>> GetUpcomingAsync(DateTime date)
    {
        var nextDay = date.AddDays(1);

        return await _db.Visits
            .Include(v => v.Patient)
            .Include(v => v.AssignedDoctor)
            .Where(v => v.ScheduledAt >= date && v.ScheduledAt < nextDay)
            .OrderBy(v => v.ScheduledAt)
            .Select(v => new VisitDto
            {
                Id = v.Id,
                PatientId = v.PatientId,
                PatientName = v.Patient.FirstName + " " + v.Patient.LastName,
                AssignedDoctorId = v.AssignedDoctorId,
                DoctorName = v.AssignedDoctor.Email!,
                ScheduledAt = v.ScheduledAt,
                Status = v.Status.ToString(),
                TotalCost = v.TotalCost,
                Notes = v.Notes
            })
            .ToListAsync();
    }

    /// <summary>
    /// Pojedyncza wizyta po ID.
    /// </summary>
    public async Task<VisitDto?> GetByIdAsync(Guid id)
    {
        var visit = await _db.Visits
            .Include(v => v.Patient)
            .Include(v => v.AssignedDoctor)
            .FirstOrDefaultAsync(v => v.Id == id);

        if (visit is null) return null;

        var dto = VisitMapper.ToDto(visit);
        dto.PatientName = $"{visit.Patient.FirstName} {visit.Patient.LastName}";
        dto.DoctorName = visit.AssignedDoctor.Email!;
        return dto;
    }

    /// <summary>
    /// Tworzy nową wizytę ze statusem Zaplanowana.
    /// </summary>
    public async Task<VisitDto> CreateAsync(CreateVisitRequest request)
    {
        _logger.LogInformation("Tworzenie wizyty dla pacjenta {PatientId}", request.PatientId);

        var visit = VisitMapper.ToEntity(request);
        visit.Id = Guid.NewGuid();
        visit.Status = VisitStatus.Zaplanowana;

        _db.Visits.Add(visit);
        await _db.SaveChangesAsync();

        // Pobierz nazwy do DTO
        var patient = await _db.Patients.FindAsync(visit.PatientId);
        var doctor = await _db.Users.FindAsync(visit.AssignedDoctorId);

        var dto = VisitMapper.ToDto(visit);
        dto.PatientName = patient != null ? $"{patient.FirstName} {patient.LastName}" : "-";
        dto.DoctorName = doctor?.Email ?? "-";
        return dto;
    }

    /// <summary>
    /// Aktualizuje datę/notatki/lekarza wizyty.
    /// </summary>
    public async Task<VisitDto> UpdateAsync(Guid id, CreateVisitRequest request)
    {
        _logger.LogInformation("Aktualizacja wizyty {VisitId}", id);

        var visit = await _db.Visits.FindAsync(id)
            ?? throw new KeyNotFoundException($"Wizyta o ID {id} nie została znaleziona");

        VisitMapper.UpdateEntity(request, visit);
        await _db.SaveChangesAsync();

        await _db.Entry(visit).Reference(v => v.Patient).LoadAsync();
        await _db.Entry(visit).Reference(v => v.AssignedDoctor).LoadAsync();

        var dto = VisitMapper.ToDto(visit);
        dto.PatientName = $"{visit.Patient.FirstName} {visit.Patient.LastName}";
        dto.DoctorName = visit.AssignedDoctor.Email!;
        return dto;
    }

    /// <summary>
    /// Zmienia status wizyty.
    /// </summary>
    public async Task<VisitDto> UpdateStatusAsync(Guid id, string status)
    {
        _logger.LogInformation("Zmiana statusu wizyty {VisitId} na {Status}", id, status);

        var visit = await _db.Visits.FindAsync(id)
            ?? throw new KeyNotFoundException($"Wizyta o ID {id} nie została znaleziona");

        if (!Enum.TryParse<VisitStatus>(status, out var newStatus))
        {
            throw new ArgumentException($"Nieprawidłowy status: {status}");
        }

        visit.Status = newStatus;

        if (newStatus == VisitStatus.Zakonczona)
        {
            visit.TotalCost = 0; // TODO: sumować z procedur i leków (moduły #US5, #US6)
        }

        await _db.SaveChangesAsync();

        await _db.Entry(visit).Reference(v => v.Patient).LoadAsync();
        await _db.Entry(visit).Reference(v => v.AssignedDoctor).LoadAsync();

        var dto = VisitMapper.ToDto(visit);
        dto.PatientName = $"{visit.Patient.FirstName} {visit.Patient.LastName}";
        dto.DoctorName = visit.AssignedDoctor.Email!;
        return dto;
    }

    /// <summary>
    /// Anuluje wizytę – ustawia status Anulowana. Nie usuwa fizycznie.
    /// </summary>
    public async Task CancelAsync(Guid id)
    {
        _logger.LogWarning("Anulowanie wizyty {VisitId}", id);

        var visit = await _db.Visits.FindAsync(id)
            ?? throw new KeyNotFoundException($"Wizyta o ID {id} nie została znaleziona");

        visit.Status = VisitStatus.Anulowana;
        await _db.SaveChangesAsync();
    }
}
