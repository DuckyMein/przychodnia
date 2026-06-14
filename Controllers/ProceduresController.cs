
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClinicManager.DTOs;
using ClinicManager.Services;

namespace ClinicManager.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Rejestratorka,Lekarz")]
public class ProceduresController : ControllerBase
{
    private readonly IProcedureService _procedureService;

    public ProceduresController(IProcedureService procedureService)
    {
        _procedureService = procedureService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProcedureDto>>> GetAll() => Ok(await _procedureService.GetAllAsync());

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProcedureDto>> GetById(Guid id)
    {
        var procedure = await _procedureService.GetByIdAsync(id);
        return procedure is null ? NotFound() : Ok(procedure);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Rejestratorka")]
    public async Task<ActionResult<ProcedureDto>> Create([FromBody] CreateProcedureRequest request)
    {
        var procedure = await _procedureService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = procedure.Id }, procedure);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Rejestratorka")]
    public async Task<ActionResult<ProcedureDto>> Update(Guid id, [FromBody] CreateProcedureRequest request)
    {
        try { return Ok(await _procedureService.UpdateAsync(id, request)); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,Rejestratorka")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try { await _procedureService.DeleteAsync(id); return NoContent(); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }
}