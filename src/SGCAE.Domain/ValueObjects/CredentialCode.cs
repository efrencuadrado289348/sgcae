using SGCAE.Domain.Common;

namespace SGCAE.Domain.ValueObjects;

/// <summary>Código único impreso/grabado en la credencial (tarjeta, QR, NFC...).</summary>
public sealed record CredentialCode
{
    public const int MaxLength = 30;

    public string Value { get; }

    private CredentialCode(string value) => Value = value;

    public static CredentialCode Create(string? value)
    {
        var normalized = Guard.Required(value, "código de credencial", MaxLength).ToUpperInvariant();
        if (normalized.Length < 6 || !normalized.All(c => char.IsLetterOrDigit(c) || c == '-'))
            throw new DomainException("El código de credencial debe tener mínimo 6 caracteres (letras, números o '-').");

        return new CredentialCode(normalized);
    }

    public override string ToString() => Value;
}
