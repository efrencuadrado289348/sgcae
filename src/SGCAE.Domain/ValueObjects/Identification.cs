using SGCAE.Domain.Common;

namespace SGCAE.Domain.ValueObjects;

/// <summary>Número de identificación del empleado (cédula, pasaporte...). Único en el sistema.</summary>
public sealed record Identification
{
    public const int MaxLength = 20;

    public string Value { get; }

    private Identification(string value) => Value = value;

    public static Identification Create(string? value)
    {
        var normalized = Guard.Required(value, "identificación", MaxLength).ToUpperInvariant();
        if (normalized.Length < 5 || !normalized.All(char.IsLetterOrDigit))
            throw new DomainException("La identificación debe tener entre 5 y 20 caracteres alfanuméricos.");

        return new Identification(normalized);
    }

    public override string ToString() => Value;
}
