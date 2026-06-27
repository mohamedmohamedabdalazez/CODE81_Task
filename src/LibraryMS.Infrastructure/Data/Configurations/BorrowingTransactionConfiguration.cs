using LibraryMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryMS.Infrastructure.Data.Configurations;

public class BorrowingTransactionConfiguration : IEntityTypeConfiguration<BorrowingTransaction>
{
    public void Configure(EntityTypeBuilder<BorrowingTransaction> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Status).HasConversion<string>();

        builder.HasOne(t => t.Book)
               .WithMany(b => b.BorrowingTransactions)
               .HasForeignKey(t => t.BookId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Member)
               .WithMany(m => m.BorrowingTransactions)
               .HasForeignKey(t => t.MemberId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.BorrowedByUser)
               .WithMany()
               .HasForeignKey(t => t.BorrowedByUserId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.ReturnedByUser)
               .WithMany()
               .HasForeignKey(t => t.ReturnedByUserId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(false);
    }
}
