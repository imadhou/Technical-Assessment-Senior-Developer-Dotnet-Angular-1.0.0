using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using N2J.Domain.Entities;

namespace N2J.Infrastructure.Persistence.Configurations;

internal sealed class RoleEntityTypeConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");
        builder.HasKey(r => r.Id)
            .HasName("PK_Roles");

        builder.Property(p => p.Id)
            .HasColumnName("ID")
            .IsRequired()
            .ValueGeneratedOnAdd()
            .HasColumnType("uuid");

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.Description)
            .HasMaxLength(250);
    }
}