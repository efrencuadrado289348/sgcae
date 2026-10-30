namespace SGCAE.Domain.Common;

/// <summary>Se lanza cuando se viola una regla o invariante del negocio.</summary>
public class DomainException(string message) : Exception(message);
