using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ClinicManager.Data;
using ClinicManager.DTOs;
using ClinicManager.Mappers;
using ClinicManager.Models;

namespace ClinicManager.Services;

/// <summary>
/// Implementacja serwisu kartoteki medycznej.
/// Każda operacja na MedicalRecord jest logowana w AuditLog (RODO).
/// </summary>
public class MedicalRecordService : IMedicalRecordService
{
    private readonly ApplicationDbContext _db;
    private readonly IHttpContextAccessor _httpContext;
    private readonly ILogger<MedicalRecordService> _logger;

    public MedicalRecordService(
        ApplicationDbContext db,
        IHttpContextAccessor httpContext,
        ILogger<MedicalRecordService> logger)
    {
        _db = db;
        _httpContext = httpContext;
        _logger = logger;
    }

    /// <summary>
    /// Pobiera wszystkie wpisy kartoteki dla danego pacjenta. Loguje odczyt.
    /// </summary>
    public async Task<List<MedicalRecordDto>> GetByPatientAsync(Guid patientId)
    {
        _logger.LogInformation("Pobieranie kartoteki pacjenta {PatientId}", patientId);

        var records = await _db.MedicalRecords
            .Where(m => m.PatientId == patientId)
            .OrderByDescending(m => m.UploadedAt)
            .ToListAsync();

        var dtos = new List<MedicalRecordDto>();
        foreach (var record in records)
        {
            var dto = MedicalRecordMapper.ToDto(record);
            // Uzupełniamy nazwę pacjenta (pomijamy w mapperze żeby uniknąć N+1)
            dto.PatientName = record.Patient != null
                ? $"{record.Patient.FirstName} {record.Patient.LastName}"
                : "-";
            dtos.Add(dto);
        }

        // Audyt RODO – logowanie odczytu
        foreach (var record in records)
        {
            await LogAuditAsync(record.Id, AuditOperation.Read, $"Odczytano kartotekę pacjenta {patientId}");
        }

        return dtos;
    }

    /// <summary>
    /// Pobiera pojedynczy wpis kartoteki. Loguje odczyt.
    /// </summary>
    public async Task<MedicalRecordDto?> GetByIdAsync(Guid id)
    {
        _logger.LogInformation("Pobieranie wpisu kartoteki {RecordId}", id);

        var record = await _db.MedicalRecords
            .Include(m => m.Patient)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (record is null) return null;

        // Audyt RODO
        await LogAuditAsync(record.Id, AuditOperation.Read, "Odczytano wpis kartoteki");

        var dto = MedicalRecordMapper.ToDto(record);
        dto.PatientName = $"{record.Patient.FirstName} {record.Patient.LastName}";
        return dto;
    }

    /// <summary>
    /// Tworzy nowy wpis w kartotece. URL skanu ustawiany po uploadzie.
    /// </summary>
    public async Task<MedicalRecordDto> CreateAsync(CreateMedicalRecordRequest request, string? fileUrl)
    {
        _logger.LogInformation("Tworzenie wpisu w kartotece dla pacjenta {PatientId}", request.PatientId);

        var record = MedicalRecordMapper.ToEntity(request);
        record.Id = Guid.NewGuid();
        record.DocumentScanUrl = fileUrl;
        record.UploadedAt = DateTime.UtcNow;

        _db.MedicalRecords.Add(record);
        await _db.SaveChangesAsync();

        // Audyt RODO
        await LogAuditAsync(record.Id, AuditOperation.Create, $"Utworzono wpis: {request.Description}");

        // Pobierz pacjenta do nazwy w DTO
        var patient = await _db.Patients.FindAsync(record.PatientId);
        var dto = MedicalRecordMapper.ToDto(record);
        dto.PatientName = patient != null ? $"{patient.FirstName} {patient.LastName}" : "-";
        return dto;
    }

    /// <summary>
    /// Aktualizuje opis wpisu. Nie zmienia URL skanu.
    /// </summary>
    public async Task<MedicalRecordDto> UpdateAsync(Guid id, CreateMedicalRecordRequest request)
    {
        _logger.LogInformation("Aktualizacja wpisu kartoteki {RecordId}", id);

        var record = await _db.MedicalRecords
            .Include(m => m.Patient)
            .FirstOrDefaultAsync(m => m.Id == id)
            ?? throw new KeyNotFoundException($"Wpis kartoteki o ID {id} nie został znaleziony");

        MedicalRecordMapper.UpdateEntity(request, record);
        await _db.SaveChangesAsync();

        // Audyt RODO
        await LogAuditAsync(record.Id, AuditOperation.Update, $"Zmodyfikowano opis wpisu");

        var dto = MedicalRecordMapper.ToDto(record);
        dto.PatientName = $"{record.Patient.FirstName} {record.Patient.LastName}";
        return dto;
    }

    /// <summary>
    /// Usuwa wpis z kartoteki. Loguje usunięcie.
    /// </summary>
    public async Task DeleteAsync(Guid id)
    {
        _logger.LogWarning("Usuwanie wpisu kartoteki {RecordId}", id);

        var record = await _db.MedicalRecords.FindAsync(id)
            ?? throw new KeyNotFoundException($"Wpis kartoteki o ID {id} nie został znaleziony");

        _db.MedicalRecords.Remove(record);

        // Audyt RODO
        await LogAuditAsync(id, AuditOperation.Delete, $"Usunięto wpis: {record.Description}");

        await _db.SaveChangesAsync();
    }

    /// <summary>
    /// Zapisuje wpis w dzienniku audytu (RODO).
    /// </summary>
    private async Task LogAuditAsync(Guid entityId, AuditOperation operation, string? details)
    {
        var userId = _httpContext.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

        var audit = new AuditLog
        {
            Id = Guid.NewGuid(),
            EntityName = nameof(MedicalRecord),
            EntityId = entityId,
            UserId = userId,
            Operation = operation,
            Timestamp = DateTime.UtcNow,
            Details = details
        };

        _db.AuditLogs.Add(audit);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Audyt RODO: {Operation} na {Entity}/{Id} przez {User}",
            operation, nameof(MedicalRecord), entityId, userId ?? "nieznany");
    }
}
