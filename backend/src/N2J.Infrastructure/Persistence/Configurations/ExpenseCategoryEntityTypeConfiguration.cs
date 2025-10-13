using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using N2J.Domain.Entities;

namespace N2J.Infrastructure.Persistence.Configurations;

internal sealed class ExpenseCategoryEntityTypeConfiguration : IEntityTypeConfiguration<ExpenseCategory>
{
    public void Configure(EntityTypeBuilder<ExpenseCategory> builder)
    {
        builder.ToTable("ExpenseCategories");
        builder.HasKey(ec => ec.Id)
            .HasName("PK_ExpenseCategories");

        builder.Property(p => p.Id)
            .HasColumnName("ID")
            .IsRequired()
            .ValueGeneratedOnAdd()
            .HasColumnType("uuid");

        builder.Property(ec => ec.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(ec => ec.Description)
            .HasMaxLength(500);

        builder.Property(ec => ec.CreatedAt)
            .HasDefaultValueSql("NOW()")
            .ValueGeneratedOnAdd();

        builder.Property(ec => ec.CompanyId)
            .IsRequired();

        builder.HasOne(ec => ec.Company)
            .WithMany(c => c.ExpenseCategories)
            .HasForeignKey(ec => ec.CompanyId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_ExpenseCategories_Companies");

    }
}