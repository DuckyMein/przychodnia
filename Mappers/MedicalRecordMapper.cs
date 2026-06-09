using Riok.Mapperly.Abstractions;
using ClinicManager.DTOs;
using ClinicManager.Models;

namespace ClinicManager.Mappers;

/// <summary>
/// Mapperly – MedicalRecord ↔ DTO.
/// </summary>
[Mapper]
public static partial class MedicalRecordMapper
{
    // MedicalRecord → MedicalRecordDto (nazwę pacjenta ustawiamy ręcznie w serwisie)
    [MapperIgnoreSource(nameof(MedicalRecord.Patient))]
    public static partial MedicalRecordDto ToDto(MedicalRecord record);

    // CreateMedicalRecordRequest → MedicalRecord
    [MapperIgnoreTarget(nameof(MedicalRecord.Id))]
    [MapperIgnoreTarget(nameof(MedicalRecord.DocumentScanUrl))]
    [MapperIgnoreTarget(nameof(MedicalRecord.Patient))]
    public static partial MedicalRecord ToEntity(CreateMedicalRecordRequest request);

    // Aktualizacja
    [MapperIgnoreTarget(nameof(MedicalRecord.Id))]
    [MapperIgnoreTarget(nameof(MedicalRecord.DocumentScanUrl))]
    [MapperIgnoreTarget(nameof(MedicalRecord.Patient))]
    [MapperIgnoreTarget(nameof(MedicalRecord.PatientId))]
    public static partial void UpdateEntity(CreateMedicalRecordRequest request, MedicalRecord record);
}
