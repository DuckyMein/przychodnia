namespace ClinicManager.Services;

public interface IReportService
{
    // Przyjmuje opcjonalne filtry i zwraca plik PDF w formie tablicy bajtów
    Task<byte[]> GenerateSummaryReportAsync(Guid? patientId, string? doctorId, int? year, int? month);
}