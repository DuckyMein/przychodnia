using ClinicManager.Data;
using ClinicManager.DTOs;
using Riok.Mapperly.Abstractions;

namespace ClinicManager.Mappers;

[Mapper]
public partial class MedicationMapper
{
    // Tłumaczy z bazy na DTO (do wyświetlania)
    public partial MedicationDto MedicationToMedicationDto(Medication medication);

    // Tłumaczy z formularza na bazę (do zapisu)
    public partial Medication CreateMedicationRequestToMedication(CreateMedicationRequest request);
}