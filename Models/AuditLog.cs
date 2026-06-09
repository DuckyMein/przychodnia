namespace ClinicManager.Models;

/// <summary>
/// Ślad audytowy (RODO) – każdy dostęp do kartoteki medycznej jest logowany.
/// </summary>
public class AuditLog
{
    public Guid Id { get; set; }

    // Która encja (np. "MedicalRecord")
    public string EntityName { get; set; } = null!;

    // ID encji
    public Guid EntityId { get; set; }

    // Kto wykonał operację (ID użytkownika Identity)
    public string? UserId { get; set; }

    // Rodzaj operacji
    public AuditOperation Operation { get; set; }

    // Kiedy
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    // Dodatkowy opis (np. co zmieniono)
    public string? Details { get; set; }
}

/// <summary>
/// Rodzaj operacji audytowanej.
/// </summary>
public enum AuditOperation
{
    Read = 1,
    Create = 2,
    Update = 3,
    Delete = 4
}
