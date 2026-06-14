using ClinicManager.Data;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ClinicManager.Services;

public class ReportService : IReportService
{
    private readonly ApplicationDbContext _context;

    public ReportService(ApplicationDbContext context)
    {
        _context = context;
        // Konfiguracja darmowej licencji dla QuestPDF
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<byte[]> GenerateSummaryReportAsync(Guid? patientId, string? doctorId, int? year, int? month)
    {
        // 1. Zaczynamy od pobrania wszystkich wizyt z bazy wraz z danymi powiązanymi
        var query = _context.Visits
            .Include(v => v.Patient)
            .Include(v => v.AssignedDoctor)
            .AsNoTracking() // AsNoTracking przyspiesza odczyt, bo to tylko raport
            .AsQueryable();

        // 2. Nakładamy filtry (tylko te, które zostały podane)
        if (patientId.HasValue)
            query = query.Where(v => v.PatientId == patientId.Value);

        if (!string.IsNullOrEmpty(doctorId))
            query = query.Where(v => v.AssignedDoctorId == doctorId);

        if (year.HasValue)
            query = query.Where(v => v.ScheduledAt.Year == year.Value);

        if (month.HasValue)
            query = query.Where(v => v.ScheduledAt.Month == month.Value);

        // 3. Pobieramy przefiltrowane dane i liczymy sumę
        var visits = await query.ToListAsync();
        var totalCost = visits.Sum(v => v.TotalCost);

        // 4. Generujemy plik PDF
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily(Fonts.Arial));

                page.Header().Text("Raport Świadczeń i Kosztów")
                    .SemiBold().FontSize(20).FontColor(Colors.Blue.Darken2);

                page.Content().PaddingVertical(10).Column(column =>
                {
                    // Nagłówek raportu
                    column.Item().Text($"Wygenerowano: {DateTime.Now:dd.MM.yyyy HH:mm}");
                    column.Item().PaddingBottom(15).Text($"Łączna suma kosztów: {totalCost:C2}")
                        .FontSize(14).SemiBold();

                    // Tabela ze szczegółami wizyt
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();  // Data
                            columns.RelativeColumn(2); // Pacjent
                            columns.RelativeColumn(2); // Lekarz
                            columns.RelativeColumn();  // Koszt
                        });

                        // Nagłówki tabeli
                        table.Header(header =>
                        {
                            header.Cell().BorderBottom(1).Padding(2).Text("Data").SemiBold();
                            header.Cell().BorderBottom(1).Padding(2).Text("Pacjent").SemiBold();
                            header.Cell().BorderBottom(1).Padding(2).Text("Lekarz").SemiBold();
                            header.Cell().BorderBottom(1).Padding(2).Text("Koszt").SemiBold();
                        });

                        // Wiersze tabeli
                        foreach (var visit in visits)
                        {
                            table.Cell().Padding(2).Text(visit.ScheduledAt.ToString("dd.MM.yyyy"));
                            table.Cell().Padding(2).Text($"{visit.Patient.FirstName} {visit.Patient.LastName}");
                            table.Cell().Padding(2).Text(visit.AssignedDoctor?.UserName ?? "Brak danych");
                            table.Cell().Padding(2).Text(visit.TotalCost.ToString("C2"));
                        }
                    });
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Strona ");
                    x.CurrentPageNumber();
                });
            });
        }).GeneratePdf();
    }
}