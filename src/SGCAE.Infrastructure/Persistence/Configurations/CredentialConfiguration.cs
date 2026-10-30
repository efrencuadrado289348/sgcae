using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGCAE.Domain.Credentials;
using SGCAE.Domain.Employees;
using SGCAE.Domain.ValueObjects;

namespace SGCAE.Infrastructure.Persistence.Configurations;

internal sealed class CredentialConfiguration : IEntityTypeConfiguration<Credential>
{
    public void Configure(EntityTypeBuilder<Credential> b)
    {
        b.ToTable("Credentials");
        b.HasKey(c => c.Id);
        b.Property(c => c.Id).ValueGeneratedNever();

        b.Property(c => c.Code)
            .HasConversion(v => v.Value, v => CredentialCode.Create(v))
            .HasMaxLength(CredentialCode.MaxLength).IsRequired();
        b.HasIndex(c => c.Code).IsUnique();

        b.Property(c => c.Status).HasConversion<string>().HasMaxLength(20);

        // FK sin propiedad de navegación: los agregados se referencian solo por Id.
        b.HasOne<Employee>().WithMany().HasForeignKey(c => c.EmployeeId).OnDelete(DeleteBehavior.Restrict);

        b.Ignore(c => c.CanBeUsed);
    }
}
