namespace SGCAE.Domain.Common;

internal static class Guard
{
    public static string Required(string? value, string field, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException($"El campo '{field}' es obligatorio.");

        var trimmed = value.Trim();
        if (trimmed.Length > maxLength)
            throw new DomainException($"El campo '{field}' no puede superar {maxLength} caracteres.");

        return trimmed;
    }
}
