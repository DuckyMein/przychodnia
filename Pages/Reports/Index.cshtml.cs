using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ClinicManager.Data;
using ClinicManager.Services;

namespace ClinicManager.Pages.Reports;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly IReportService _reportService;

    public IndexModel(ApplicationDbContext context, IReportService reportService)
    {
        _context = context;
        _reportService = reportService;
    }

    // Pola przechowujące wybór użytkownika
    [BindProperty] public Guid? SelectedPatientId { get; set; }
    [BindProperty] public string? SelectedDoctorId { get; set; }
    [BindProperty] public int? SelectedYear { get; set; }
    [BindProperty] public int? SelectedMonth { get; set; }

    // Listy rozwijane do widoku
    public SelectList Patients { get; set; } = default!;
    public SelectList Doctors { get; set; } = default!;
    public SelectList Years { get; set; } = default!;
    public SelectList Months { get; set; } = default!;

    public async Task OnGetAsync()
    {
        // Pobieramy pacjentów
        var patients = await _context.Patients
            .Select(p => new { p.Id, FullName = p.FirstName + " " + p.LastName })
            .ToListAsync();
        Patients = new SelectList(patients, "Id", "FullName");

        // Pobieramy lekarzy (z tabeli AspNetUsers)
        var doctors = await _context.Users
            .Select(u => new { u.Id, FullName = u.UserName })
            .ToListAsync();
        Doctors = new SelectList(doctors, "Id", "FullName");

        // Przygotowujemy listę lat (od 5 lat wstecz do teraz)
        Years = new SelectList(Enumerable.Range(DateTime.Now.Year - 5, 6));

        // Przygotowujemy listę miesięcy
        var months = Enumerable.Range(1, 12).Select(m => new { Value = m, Text = new DateTime(2000, m, 1).ToString("MMMM") });
        Months = new SelectList(months, "Value", "Text");
    }

    // Ta metoda odpala się, gdy klikniemy "Pobierz"
    public async Task<IActionResult> OnPostDownloadAsync()
    {
        // 1. Zlecamy serwisowi wygenerowanie PDFa (przekazując opcjonalne filtry)
        var pdfBytes = await _reportService.GenerateSummaryReportAsync(SelectedPatientId, SelectedDoctorId, SelectedYear, SelectedMonth);

        // 2. Nadajemy plikowi nazwę z obecną datą
        string fileName = $"Raport_{DateTime.Now:yyyyMMdd_HHmm}.pdf";

        // 3. Zwracamy plik do pobrania w przeglądarce
        return File(pdfBytes, "application/pdf", fileName);
    }
}