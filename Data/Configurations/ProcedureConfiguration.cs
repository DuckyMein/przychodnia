using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ClinicManager.Models;

namespace ClinicManager.Data.Configurations;

public class ProcedureConfiguration : IEntityTypeConfiguration<Procedure>
{
	public void Configure(EntityTypeBuilder<Procedure> builder)
	{
		builder.ToTable("Procedures");
		builder.HasKey(p => p.Id);
		builder.Property(p => p.Id).HasDefaultValueSql("NEWSEQUENTIALID()");

		builder.Property(p => p.Description).IsRequired().HasMaxLength(250);
		builder.Property(p => p.ServiceCost).IsRequired().HasColumnType("decimal(10,2)");
	}
}
