using ClinicManager.Data;
using ClinicManager.DTOs;
using ClinicManager.Mappers;
using Microsoft.EntityFrameworkCore;

namespace ClinicManager.Services;

public class MedicationService : IMedicationService
{
    private readonly ApplicationDbContext _context;
    private readonly MedicationMapper _mapper;

    public MedicationService(ApplicationDbContext context)
    {
        _context = context;
        _mapper = new MedicationMapper();
    }

    public async Task<IEnumerable<MedicationDto>> GetAllAsync()
    {
        var medications = await _context.Medications.ToListAsync();
        return medications.Select(m => _mapper.MedicationToMedicationDto(m)).ToList();
    }

    public async Task<Guid> CreateAsync(CreateMedicationRequest request)
    {
        var medication = _mapper.CreateMedicationRequestToMedication(request);

        _context.Medications.Add(medication);
        await _context.SaveChangesAsync();

        return medication.Id;
    }
}