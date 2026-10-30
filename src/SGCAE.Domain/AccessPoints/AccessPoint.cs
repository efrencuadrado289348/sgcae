using SGCAE.Domain.Common;

namespace SGCAE.Domain.AccessPoints;

/// <summary>Agregado Punto de acceso (RF-06): recepción, parqueadero, bodega, área restringida.</summary>
public sealed class AccessPoint : AggregateRoot
{
    public const int CodeMaxLength = 20;
    public const int NameMaxLength = 100;

    /// <summary>Código corto y único que envía el lector físico, p. ej. "BOD-01".</summary>
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public AccessPointType Type { get; private set; }
    public bool IsEnabled { get; private set; }

    private AccessPoint() { }

    public static AccessPoint Create(string code, string name, AccessPointType type)
    {
        if (!Enum.IsDefined(type))
            throw new DomainException($"El tipo de punto de acceso '{type}' no es válido.");

        return new AccessPoint
        {
            Code = NormalizeCode(code),
            Name = Guard.Required(name, "nombre", NameMaxLength),
            Type = type,
            IsEnabled = true
        };
    }

    /// <summary>Regla única de normalización: la usan el agregado y las búsquedas por código.</summary>
    public static string NormalizeCode(string? code) =>
        Guard.Required(code, "código", CodeMaxLength).ToUpperInvariant();

    public void Enable()
    {
        if (IsEnabled)
            throw new DomainException("El punto de acceso ya está habilitado.");

        IsEnabled = true;
    }

    public void Disable()
    {
        if (!IsEnabled)
            throw new DomainException("El punto de acceso ya está deshabilitado.");

        IsEnabled = false;
    }
}
