using ClinicManager.Data;
using ClinicManager.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ClinicManager.Services;

public class PrescriptionService : IPrescriptionService
{
    private readonly ApplicationDbContext _context;

    public PrescriptionService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PrescriptionDto>> GetAllAsync()
    {
        // Pobieramy recepty wraz z danymi Pacjenta i elementami (lekami)
        var prescriptions = await _context.Prescriptions
            .Include(p => p.Patient)
            .Include(p => p.PrescriptionItems)
                .ThenInclude(i => i.Medication)
            .OrderByDescending(p => p.IssueDate)
            .ToListAsync();

        // Przepisujemy do DTO
        return prescriptions.Select(p => new PrescriptionDto
        {
            Id = p.Id,
            AccessCode = p.AccessCode,
            IssueDate = p.IssueDate,
            ExpiryDate = p.ExpiryDate,
            PatientName = p.Patient != null ? $"{p.Patient.FirstName} {p.Patient.LastName}" : "Brak danych",
            Items = p.PrescriptionItems.Select(i => new PrescriptionItemDto
            {
                MedicationName = i.Medication != null ? i.Medication.Name : "Nieznany lek",
                Dosage = i.Dosage,
                Quantity = i.Quantity
            }).ToList()
        });
    }

    public async Task<Guid> CreateAsync(CreatePrescriptionRequest request)
    {
        // Generujemy losowy 4-cyfrowy PIN
        var random = new Random();
        var pinCode = random.Next(1000, 10000).ToString();

        var prescription = new Prescription
        {
            Id = Guid.NewGuid(),
            PatientId = request.PatientId,
            AccessCode = pinCode,
            IssueDate = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddDays(30),
            PrescriptionItems = request.Items.Select(i => new PrescriptionItem
            {
                Id = Guid.NewGuid(),
                MedicationId = i.MedicationId,
                Dosage = i.Dosage,
                Quantity = i.Quantity
            }).ToList()
        };

        _context.Prescriptions.Add(prescription);
        await _context.SaveChangesAsync();

        return prescription.Id;
    }
}
