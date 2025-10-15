using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using N2J.Domain.Entities;

namespace N2J.Infrastructure.Persistence.Configurations;

internal sealed class ExpenseEntityTypeConfiguration : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        builder.ToTable("Expenses");
        builder.HasKey(e => e.Id)
            .HasName("PK_Expenses");

        builder.Property(p => p.Id)
            .HasColumnName("ID")
            .IsRequired()
            .ValueGeneratedOnAdd()
            .HasColumnType("uuid");

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.Amount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(e => e.Date)
            .IsRequired();

        builder.Property(e => e.Status)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("Pending");

        builder.Property(e => e.ReceiptUrl)
            .HasMaxLength(2048);

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        builder.Property(e => e.UpdatedAt)
            .IsRequired(false);

        builder.Property(e => e.CreatedBy)
            .IsRequired()
            .HasColumnType("uuid");

        builder.Property(e => e.UpdatedBy)
            .HasColumnType("uuid");

        builder.Property(e => e.ExpenseCategoryId)
            .IsRequired();

        builder.Property(e => e.UserId)
            .IsRequired();

        // Relationships
        builder.HasOne(e => e.ExpenseCategory)
            .WithMany(c => c.Expenses)
            .HasForeignKey(e => e.ExpenseCategoryId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Expenses_ExpenseCategories");

        builder.HasOne(e => e.User)
            .WithMany(u => u.Expenses)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Expenses_Users");
    }
}