using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ClinicManager.Models;

namespace ClinicManager.Data.Configurations;

public class VisitConfiguration : IEntityTypeConfiguration<Visit>
{
    public void Configure(EntityTypeBuilder<Visit> builder)
    {
        builder.ToTable("Visits");

        builder.HasKey(v => v.Id);
        builder.Property(v => v.Id).HasDefaultValueSql("NEWSEQUENTIALID()");

        // Relacja z Patient
        builder.HasOne(v => v.Patient)
            .WithMany()
            .HasForeignKey(v => v.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relacja z ApplicationUser (lekarz) – Identity używa string jako PK
        builder.HasOne(v => v.AssignedDoctor)
            .WithMany()
            .HasForeignKey(v => v.AssignedDoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(v => v.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(v => v.TotalCost).HasColumnType("decimal(10,2)");
        builder.Property(v => v.Notes).HasMaxLength(1000);

        // Indeks na datę wizyty (częste filtrowanie – np. wizyty na dziś/jutro)
        builder.HasIndex(v => v.ScheduledAt)
            .HasDatabaseName("IX_Visits_ScheduledAt")
            .IsClustered(false);

        // Indeks na status (częste filtrowanie aktywnych wizyt)
        builder.HasIndex(v => v.Status)
            .HasDatabaseName("IX_Visits_Status")
            .IsClustered(false);
    }
}
