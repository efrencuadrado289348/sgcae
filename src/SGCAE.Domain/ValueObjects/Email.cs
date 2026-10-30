using System.Text.RegularExpressions;
using SGCAE.Domain.Common;

namespace SGCAE.Domain.ValueObjects;

/// <summary>Objeto de valor: se define por su valor, es inmutable y siempre válido.</summary>
public sealed record Email
{
    public const int MaxLength = 150;
    private static readonly Regex Pattern = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    public string Value { get; }

    private Email(string value) => Value = value;

    public static Email Create(string? value)
    {
        var normalized = Guard.Required(value, "correo", MaxLength).ToLowerInvariant();
        if (!Pattern.IsMatch(normalized))
            throw new DomainException($"El correo '{value}' no tiene un formato válido.");

        return new Email(normalized);
    }

    public override string ToString() => Value;
}
