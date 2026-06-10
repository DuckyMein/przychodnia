using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClinicManager.DTOs;
using ClinicManager.Services;

namespace ClinicManager.Controllers;

/// <summary>
/// Kontroler API dla wizyt.
/// Dostęp: Admin, Rejestratorka, Lekarz.
/// Zawiera endpointy dla NBomber (GET /active) i SQL Profiler (GET /today).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Rejestratorka,Lekarz")]
public class VisitsController : ControllerBase
{
    private readonly IVisitService _visitService;
    private readonly ILogger<VisitsController> _logger;

    public VisitsController(IVisitService visitService, ILogger<VisitsController> logger)
    {
        _visitService = visitService;
        _logger = logger;
    }

    /// <summary>
    /// GET /api/visits – wszystkie wizyty
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<VisitDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<VisitDto>>> GetAll()
    {
        var visits = await _visitService.GetAllAsync();
        return Ok(visits);
    }

    /// <summary>
    /// GET /api/visits/today – wizyty na dziś (SQL Profiler endpoint)
    /// </summary>
    [HttpGet("today")]
    [ProducesResponseType(typeof(List<VisitDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<VisitDto>>> GetToday()
    {
        var visits = await _visitService.GetTodayAsync();
        return Ok(visits);
    }

    /// <summary>
    /// GET /api/visits/active – aktywne wizyty z JOIN-ami (NBomber endpoint)
    /// </summary>
    [HttpGet("active")]
    [ProducesResponseType(typeof(List<VisitListResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<VisitListResponse>>> GetActive()
    {
        var visits = await _visitService.GetActiveAsync();
        return Ok(visits);
    }

    /// <summary>
    /// GET /api/visits/patient/{patientId} – wizyty pacjenta
    /// </summary>
    [HttpGet("patient/{patientId:guid}")]
    [ProducesResponseType(typeof(List<VisitDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<VisitDto>>> GetByPatient(Guid patientId)
    {
        var visits = await _visitService.GetByPatientAsync(patientId);
        return Ok(visits);
    }

    /// <summary>
    /// GET /api/visits/doctor/{doctorId} – wizyty lekarza
    /// </summary>
    [HttpGet("doctor/{doctorId}")]
    [ProducesResponseType(typeof(List<VisitDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<VisitDto>>> GetByDoctor(string doctorId)
    {
        var visits = await _visitService.GetByDoctorAsync(doctorId);
        return Ok(visits);
    }

    /// <summary>
    /// GET /api/visits/upcoming/{date} – wizyty na podany dzień (BackgroundService)
    /// </summary>
    [HttpGet("upcoming/{date:datetime}")]
    [ProducesResponseType(typeof(List<VisitDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<VisitDto>>> GetUpcoming(DateTime date)
    {
        var visits = await _visitService.GetUpcomingAsync(date);
        return Ok(visits);
    }

    /// <summary>
    /// GET /api/visits/{id} – szczegóły wizyty
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(VisitDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VisitDto>> GetById(Guid id)
    {
        var visit = await _visitService.GetByIdAsync(id);
        if (visit is null)
        {
            return NotFound(new { message = $"Wizyta o ID {id} nie została znaleziona" });
        }
        return Ok(visit);
    }

    /// <summary>
    /// POST /api/visits – tworzenie nowej wizyty (domyślnie: Zaplanowana)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(VisitDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<VisitDto>> Create([FromBody] CreateVisitRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var visit = await _visitService.CreateAsync(request);
        _logger.LogInformation("Utworzono wizytę {VisitId}", visit.Id);

        return CreatedAtAction(nameof(GetById), new { id = visit.Id }, visit);
    }

    /// <summary>
    /// PUT /api/visits/{id} – aktualizacja wizyty
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(VisitDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VisitDto>> Update(Guid id, [FromBody] CreateVisitRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var visit = await _visitService.UpdateAsync(id, request);
            return Ok(visit);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// PATCH /api/visits/{id}/status – zmiana statusu wizyty
    /// </summary>
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(VisitDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VisitDto>> UpdateStatus(Guid id, [FromBody] UpdateVisitStatusRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var visit = await _visitService.UpdateStatusAsync(id, request.Status);
            return Ok(visit);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// DELETE /api/visits/{id} – anulowanie wizyty
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(Guid id)
    {
        try
        {
            await _visitService.CancelAsync(id);
            _logger.LogWarning("Anulowano wizytę {VisitId}", id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
