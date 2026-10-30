using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGCAE.Domain.AccessRecords;
using SGCAE.Domain.ValueObjects;

namespace SGCAE.Infrastructure.Persistence.Configurations;

/// <summary>
/// Tabla de auditoría: SIN llaves foráneas a propósito, porque se registran también intentos
/// con credenciales o puntos inexistentes. Los índices soportan los filtros del RF-04.
/// </summary>
internal sealed class AccessRecordConfiguration : IEntityTypeConfiguration<AccessRecord>
{
    public void Configure(EntityTypeBuilder<AccessRecord> b)
    {
        b.ToTable("AccessRecords");
        b.HasKey(r => r.Id);
        b.Property(r => r.Id).ValueGeneratedNever();
        b.Property(r => r.CredentialCode).HasMaxLength(CredentialCode.MaxLength + 20).IsRequired();
        b.Property(r => r.AccessPointCode).HasMaxLength(50).IsRequired();
        b.Property(r => r.MovementType).HasConversion<string>().HasMaxLength(10);
        b.Property(r => r.Result).HasConversion<string>().HasMaxLength(15);
        b.Property(r => r.RejectionReason).HasConversion<string>().HasMaxLength(40);

        b.HasIndex(r => r.OccurredAtUtc);
        b.HasIndex(r => new { r.EmployeeId, r.OccurredAtUtc });
        b.HasIndex(r => new { r.AccessPointId, r.OccurredAtUtc });
    }
}
