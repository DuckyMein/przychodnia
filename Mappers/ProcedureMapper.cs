using Riok.Mapperly.Abstractions;
using ClinicManager.Models;
using ClinicManager.DTOs;

namespace ClinicManager.Mappers;

[Mapper]
public partial class ProcedureMapper
{
	public partial ProcedureDto ProcedureToDto(Procedure procedure);
	public partial Procedure CreateRequestToProcedure(CreateProcedureRequest request);
	public partial void UpdateProcedure(CreateProcedureRequest request, Procedure procedure);
}