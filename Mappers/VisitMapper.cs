using Riok.Mapperly.Abstractions;
using ClinicManager.DTOs;
using ClinicManager.Models;

namespace ClinicManager.Mappers;

[Mapper]
public static partial class VisitMapper
{
    // Visit → VisitDto (nazwy uzupełniane ręcznie w serwisie)
    [MapperIgnoreSource(nameof(Visit.Patient))]
    [MapperIgnoreSource(nameof(Visit.AssignedDoctor))]
    [MapProperty(nameof(Visit.Status), nameof(VisitDto.Status), Use = nameof(MapStatusToString))]
    public static partial VisitDto ToDto(Visit visit);

    // Visit → VisitListResponse
    [MapperIgnoreSource(nameof(Visit.Patient))]
    [MapperIgnoreSource(nameof(Visit.AssignedDoctor))]
    [MapperIgnoreSource(nameof(Visit.TotalCost))]
    [MapperIgnoreSource(nameof(Visit.Notes))]
    [MapperIgnoreSource(nameof(Visit.PatientId))]
    [MapperIgnoreSource(nameof(Visit.AssignedDoctorId))]
    [MapProperty(nameof(Visit.Status), nameof(VisitListResponse.Status), Use = nameof(MapStatusToString))]
    public static partial VisitListResponse ToListResponse(Visit visit);

    // CreateVisitRequest → Visit
    [MapperIgnoreTarget(nameof(Visit.Id))]
    [MapperIgnoreTarget(nameof(Visit.Status))]
    [MapperIgnoreTarget(nameof(Visit.TotalCost))]
    [MapperIgnoreTarget(nameof(Visit.Patient))]
    [MapperIgnoreTarget(nameof(Visit.AssignedDoctor))]
    public static partial Visit ToEntity(CreateVisitRequest request);

    // Aktualizacja
    [MapperIgnoreTarget(nameof(Visit.Id))]
    [MapperIgnoreTarget(nameof(Visit.Status))]
    [MapperIgnoreTarget(nameof(Visit.TotalCost))]
    [MapperIgnoreTarget(nameof(Visit.Patient))]
    [MapperIgnoreTarget(nameof(Visit.AssignedDoctor))]
    public static partial void UpdateEntity(CreateVisitRequest request, Visit visit);

    // Enum → string
    private static string MapStatusToString(VisitStatus status) => status.ToString();
}
