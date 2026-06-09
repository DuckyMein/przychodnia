using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClinicManager.DTOs;
using ClinicManager.Services;

namespace ClinicManager.Controllers;

/// <summary>
/// Kontroler API dla kartoteki medycznej.
/// Dostęp: Admin, Rejestratorka, Lekarz.
/// Każda operacja logowana w AuditLog (RODO).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Rejestratorka,Lekarz")]
public class MedicalRecordsController : ControllerBase
{
    private readonly IMedicalRecordService _medicalRecordService;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<MedicalRecordsController> _logger;

    // Dozwolone rozszerzenia plików
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".pdf", ".jpg", ".jpeg", ".png", ".doc", ".docx"
    };

    public MedicalRecordsController(
        IMedicalRecordService medicalRecordService,
        IWebHostEnvironment env,
        ILogger<MedicalRecordsController> logger)
    {
        _medicalRecordService = medicalRecordService;
        _env = env;
        _logger = logger;
    }

    /// <summary>
    /// GET /api/medicalrecords?patientId=... – kartoteka pacjenta
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<MedicalRecordDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<MedicalRecordDto>>> GetByPatient([FromQuery] Guid patientId)
    {
        if (patientId == Guid.Empty)
        {
            return BadRequest(new { message = "Podaj patientId" });
        }

        var records = await _medicalRecordService.GetByPatientAsync(patientId);
        return Ok(records);
    }

    /// <summary>
    /// GET /api/medicalrecords/{id} – szczegóły wpisu
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(MedicalRecordDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MedicalRecordDto>> GetById(Guid id)
    {
        var record = await _medicalRecordService.GetByIdAsync(id);
        if (record is null)
        {
            return NotFound(new { message = $"Wpis kartoteki o ID {id} nie został znaleziony" });
        }
        return Ok(record);
    }

    /// <summary>
    /// POST /api/medicalrecords – tworzenie wpisu z opcjonalnym uploadem skanu
    /// </summary>
    [HttpPost]
    [RequestSizeLimit(10 * 1024 * 1024)] // max 10 MB
    [ProducesResponseType(typeof(MedicalRecordDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MedicalRecordDto>> Create(
        [FromForm] CreateMedicalRecordRequest request,
        IFormFile? file)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        string? fileUrl = null;

        // Upload pliku jeśli dołączony
        if (file is not null && file.Length > 0)
        {
            var ext = Path.GetExtension(file.FileName);
            if (!AllowedExtensions.Contains(ext))
            {
                return BadRequest(new { message = $"Niedozwolony format pliku: {ext}. Dozwolone: .pdf, .jpg, .jpeg, .png, .doc, .docx" });
            }

            // Zapis do /wwwroot/uploads/
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            fileUrl = $"/uploads/{fileName}";
            _logger.LogInformation("Zapisano skan: {FileName}", fileName);
        }

        var record = await _medicalRecordService.CreateAsync(request, fileUrl);
        _logger.LogInformation("Utworzono wpis w kartotece {RecordId}", record.Id);

        return CreatedAtAction(nameof(GetById), new { id = record.Id }, record);
    }

    /// <summary>
    /// PUT /api/medicalrecords/{id} – aktualizacja opisu wpisu
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(MedicalRecordDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MedicalRecordDto>> Update(Guid id, [FromBody] CreateMedicalRecordRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var record = await _medicalRecordService.UpdateAsync(id, request);
            return Ok(record);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// DELETE /api/medicalrecords/{id} – usunięcie wpisu
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _medicalRecordService.DeleteAsync(id);
            _logger.LogWarning("Usunięto wpis kartoteki {RecordId}", id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
