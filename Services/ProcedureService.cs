using Microsoft.EntityFrameworkCore;
using ClinicManager.Data;
using ClinicManager.DTOs;
using ClinicManager.Mappers;

namespace ClinicManager.Services;

public class ProcedureService : IProcedureService
{
    private readonly ApplicationDbContext _context;
    private readonly ProcedureMapper _mapper;

    public ProcedureService(ApplicationDbContext context)
    {
        _context = context;
        _mapper = new ProcedureMapper();
    }

    public async Task<List<ProcedureDto>> GetAllAsync()
    {
        var procedures = await _context.Procedures.AsNoTracking().ToListAsync();
        return procedures.Select(p => _mapper.ProcedureToDto(p)).ToList();
    }

    public async Task<ProcedureDto?> GetByIdAsync(Guid id)
    {
        var procedure = await _context.Procedures.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        return procedure is null ? null : _mapper.ProcedureToDto(procedure);
    }

    public async Task<ProcedureDto> CreateAsync(CreateProcedureRequest request)
    {
        var procedure = _mapper.CreateRequestToProcedure(request);
        _context.Procedures.Add(procedure);
        await _context.SaveChangesAsync();
        return _mapper.ProcedureToDto(procedure);
    }

    public async Task<ProcedureDto> UpdateAsync(Guid id, CreateProcedureRequest request)
    {
        var procedure = await _context.Procedures.FindAsync(id);
        if (procedure is null) throw new KeyNotFoundException($"Procedura {id} nie znaleziona.");

        _mapper.UpdateProcedure(request, procedure);
        await _context.SaveChangesAsync();
        return _mapper.ProcedureToDto(procedure);
    }

    public async Task DeleteAsync(Guid id)
    {
        var procedure = await _context.Procedures.FindAsync(id);
        if (procedure is null) throw new KeyNotFoundException($"Procedura {id} nie znaleziona.");

        _context.Procedures.Remove(procedure);
        await _context.SaveChangesAsync();
    }
}