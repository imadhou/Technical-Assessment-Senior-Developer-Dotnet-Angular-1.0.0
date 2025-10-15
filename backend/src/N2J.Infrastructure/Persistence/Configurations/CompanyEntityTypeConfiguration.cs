using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using N2J.Domain.Entities;

namespace N2J.Infrastructure.Persistence.Configurations;

internal sealed class CompanyEntityTypeConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("Companies");
        builder.HasKey(c => c.Id).HasName("PK_Company");

        builder.Property(p => p.Id)
            .HasColumnName("ID")
            .IsRequired()
            .ValueGeneratedOnAdd()
            .HasColumnType("uuid");

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Domain)
           .IsRequired()
           .HasMaxLength(200);

        builder.Property(c => c.Address)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Currency)
            .IsRequired()
            .HasMaxLength(3)
            .HasDefaultValue("EUR");

        builder.Property(c => c.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        builder.Property(c => c.UpdatedAt)
            .IsRequired(false);

        // Relationships
        builder.HasMany(c => c.Users)
            .WithOne(u => u.Company)
            .HasForeignKey(u => u.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);


        builder.HasMany(c => c.ExpenseCategories)
            .WithOne(ec => ec.Company)
            .HasForeignKey(ec => ec.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}