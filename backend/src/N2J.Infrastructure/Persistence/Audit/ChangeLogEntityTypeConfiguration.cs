using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace N2J.Infrastructure.Persistence.Audit
{
    internal sealed class ChangeLogEntityTypeConfiguration : IEntityTypeConfiguration<ChangeLog>
    {
        public void Configure(EntityTypeBuilder<ChangeLog> builder)
        {
            builder.ToTable("ChangeLogs");
            builder.HasKey(c => c.Id).HasName("PK_ChangeLog");

            builder.Property(p => p.Id)
                .HasColumnName("ID")
                .IsRequired()
                .ValueGeneratedOnAdd()
                .HasColumnType("uuid");
            // Entity Information
            builder.Property(c => c.EntityName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.EntityId)
                .IsRequired()
                .HasColumnType("uuid");

            // Change Information
            builder.Property(c => c.ChangeType)
                .IsRequired()
                .HasMaxLength(20)
                .HasConversion(
                    v => v.ToUpper(),
                    v => v
                );

            builder.Property(c => c.ChangedBy)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(c => c.ChangedAt)
                .IsRequired();

            builder.Property(c => c.ChangesJson)
                .HasColumnType("jsonb");

            builder.HasIndex(c => c.EntityName, "IX_ChangeLog_EntityName");
            builder.HasIndex(c => c.EntityId, "IX_ChangeLog_EntityId");
            builder.HasIndex(c => c.ChangeType, "IX_ChangeLog_ChangeType");
            builder.HasIndex(c => c.ChangedBy, "IX_ChangeLog_ChangedBy");
            builder.HasIndex(c => c.ChangedAt, "IX_ChangeLog_ChangedAt");

            // Configure JSON properties for EF Core
            builder.Property(c => c.ChangesJson)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => v == null ? null : System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                    v => v == null ? null : System.Text.Json.JsonSerializer.Deserialize<string>(v, (System.Text.Json.JsonSerializerOptions?)null)
            );
        }
    }
}
