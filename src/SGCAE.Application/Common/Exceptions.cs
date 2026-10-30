namespace SGCAE.Application.Common;

/// <summary>El recurso solicitado no existe (HTTP 404).</summary>
public sealed class NotFoundException(string resource, object key)
    : Exception($"{resource} '{key}' no fue encontrado.");

/// <summary>La operación choca con datos existentes, p. ej. duplicados (HTTP 409).</summary>
public sealed class ConflictException(string message) : Exception(message);
