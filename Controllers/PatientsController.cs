using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClinicManager.DTOs;
using ClinicManager.Services;

namespace ClinicManager.Controllers;

/// <summary>
/// Kontroler API dla pacjentów.
/// Dostęp: Admin, Rejestratorka, Lekarz.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Rejestratorka,Lekarz")]
public class PatientsController : ControllerBase
{
    private readonly IPatientService _patientService;
    private readonly ILogger<PatientsController> _logger;

    public PatientsController(IPatientService patientService, ILogger<PatientsController> logger)
    {
        _patientService = patientService;
        _logger = logger;
    }

    /// <summary>
    /// GET /api/patients – lista wszystkich pacjentów
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<PatientDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<PatientDto>>> GetAll()
    {
        var patients = await _patientService.GetAllAsync();
        return Ok(patients);
    }

    /// <summary>
    /// GET /api/patients/{id} – szczegóły pacjenta
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PatientDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PatientDto>> GetById(Guid id)
    {
        var patient = await _patientService.GetByIdAsync(id);
        if (patient is null)
        {
            return NotFound(new { message = $"Pacjent o ID {id} nie został znaleziony" });
        }
        return Ok(patient);
    }

    /// <summary>
    /// GET /api/patients/search?pesel=...&lastName=... – wyszukiwanie pacjentów
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(List<PatientListResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<PatientListResponse>>> Search(
        [FromQuery] string? pesel,
        [FromQuery] string? lastName)
    {
        if (string.IsNullOrWhiteSpace(pesel) && string.IsNullOrWhiteSpace(lastName))
        {
            return BadRequest(new { message = "Podaj PESEL lub nazwisko do wyszukania" });
        }

        var patients = await _patientService.SearchAsync(pesel, lastName);
        return Ok(patients);
    }

    /// <summary>
    /// POST /api/patients – tworzenie nowego pacjenta
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(PatientDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PatientDto>> Create([FromBody] CreatePatientRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var patient = await _patientService.CreateAsync(request);
        _logger.LogInformation("Utworzono pacjenta {PatientId}", patient.Id);

        return CreatedAtAction(nameof(GetById), new { id = patient.Id }, patient);
    }

    /// <summary>
    /// PUT /api/patients/{id} – aktualizacja pacjenta
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(PatientDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PatientDto>> Update(Guid id, [FromBody] CreatePatientRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var patient = await _patientService.UpdateAsync(id, request);
            return Ok(patient);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// DELETE /api/patients/{id} – soft delete pacjenta
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _patientService.DeleteAsync(id);
            _logger.LogWarning("Usunięto pacjenta (soft delete) {PatientId}", id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
