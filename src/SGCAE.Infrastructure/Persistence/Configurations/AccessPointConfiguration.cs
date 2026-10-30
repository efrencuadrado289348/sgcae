using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGCAE.Domain.AccessPoints;

namespace SGCAE.Infrastructure.Persistence.Configurations;

internal sealed class AccessPointConfiguration : IEntityTypeConfiguration<AccessPoint>
{
    public void Configure(EntityTypeBuilder<AccessPoint> b)
    {
        b.ToTable("AccessPoints");
        b.HasKey(p => p.Id);
        b.Property(p => p.Id).ValueGeneratedNever();
        b.Property(p => p.Code).HasMaxLength(AccessPoint.CodeMaxLength).IsRequired();
        b.HasIndex(p => p.Code).IsUnique();
        b.Property(p => p.Name).HasMaxLength(AccessPoint.NameMaxLength).IsRequired();
        b.Property(p => p.Type).HasConversion<string>().HasMaxLength(30);
    }
}
