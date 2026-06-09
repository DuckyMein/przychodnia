using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ClinicManager.Models;

namespace ClinicManager.Data.Configurations;

public class MedicalRecordConfiguration : IEntityTypeConfiguration<MedicalRecord>
{
    public void Configure(EntityTypeBuilder<MedicalRecord> builder)
    {
        builder.ToTable("MedicalRecords");

        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.Property(m => m.Description).HasMaxLength(500).IsRequired();
        builder.Property(m => m.DocumentScanUrl).HasMaxLength(500);

        // Relacja z Patient
        builder.HasOne(m => m.Patient)
            .WithMany() // Patient jeszcze nie ma kolekcji MedicalRecords
            .HasForeignKey(m => m.PatientId)
            .OnDelete(DeleteBehavior.Restrict); // nie kasuj kaskadowo
    }
}
