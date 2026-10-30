using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGCAE.Domain.Employees;
using SGCAE.Domain.ValueObjects;

namespace SGCAE.Infrastructure.Persistence.Configurations;

/// <summary>El mapeo vive en Infraestructura: el dominio no sabe nada de EF Core.</summary>
internal sealed class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> b)
    {
        b.ToTable("Employees");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).ValueGeneratedNever(); // el Id lo genera el dominio

        // Objeto de valor -> columna simple mediante conversión.
        b.Property(e => e.Identification)
            .HasConversion(v => v.Value, v => Identification.Create(v))
            .HasMaxLength(Identification.MaxLength).IsRequired();
        b.HasIndex(e => e.Identification).IsUnique();

        b.Property(e => e.Email)
            .HasConversion(v => v.Value, v => Email.Create(v))
            .HasMaxLength(Email.MaxLength).IsRequired();
        b.HasIndex(e => e.Email).IsUnique();

        b.Property(e => e.FirstName).HasMaxLength(Employee.NameMaxLength).IsRequired();
        b.Property(e => e.LastName).HasMaxLength(Employee.NameMaxLength).IsRequired();
        b.Property(e => e.Position).HasMaxLength(Employee.PositionMaxLength).IsRequired();
        b.Property(e => e.Area).HasMaxLength(Employee.AreaMaxLength).IsRequired();
        b.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);

        b.Ignore(e => e.IsActive);
    }
}
