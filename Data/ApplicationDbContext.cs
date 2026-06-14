using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ClinicManager.Models;

namespace ClinicManager.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
{
    // Encje domenowe
    public DbSet<Patient> Patients { get; set; } = null!;
    public DbSet<MedicalRecord> MedicalRecords { get; set; } = null!;
    public DbSet<AuditLog> AuditLogs { get; set; } = null!;
    public DbSet<Visit> Visits { get; set; } = null!;
    public DbSet<Procedure> Procedures { get; set; } = null!;

    // W przyszłości:
   
    // public DbSet<Medication> Medications { get; set; }
    // public DbSet<ClinicalNote> ClinicalNotes { get; set; }
    // public DbSet<ProcedurePerformed> ProceduresPerformed { get; set; }
    // public DbSet<PrescribedMedication> PrescribedMedications { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder); // Konfiguracja Identity (AspNetUsers, AspNetRoles, ...)

        // Konfiguracje encji domenowych
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
