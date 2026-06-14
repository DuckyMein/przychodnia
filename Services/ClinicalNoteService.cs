using ClinicManager.Data;
using ClinicManager.DTOs;
using ClinicManager.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicManager.Services;

public class ClinicalNoteService : IClinicalNoteService
{
    private readonly ApplicationDbContext _context;

    public ClinicalNoteService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ClinicalNoteDto>> GetByPatientIdAsync(Guid patientId)
    {
        var notes = await _context.ClinicalNotes
            .Include(n => n.Patient)
            .Where(n => n.PatientId == patientId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

        return notes.Select(n => new ClinicalNoteDto
        {
            Id = n.Id,
            Content = n.Content,
            CreatedAt = n.CreatedAt,
            PatientName = $"{n.Patient.FirstName} {n.Patient.LastName}"
        });
    }

    public async Task<Guid> CreateAsync(CreateClinicalNoteRequest request)
    {
        var note = new ClinicalNote
        {
            Id = Guid.NewGuid(),
            PatientId = request.PatientId,
            Content = request.Content,
            CreatedAt = DateTime.UtcNow
        };

        _context.ClinicalNotes.Add(note);
        await _context.SaveChangesAsync();

        return note.Id;
    }
}