using LibraryMS.Domain.Entities;
using LibraryMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryMS.Infrastructure.Data.Configurations;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Title).IsRequired().HasMaxLength(500);
        builder.Property(b => b.ISBN).IsRequired().HasMaxLength(20);
        builder.HasIndex(b => b.ISBN).IsUnique();
        builder.Property(b => b.Edition).HasMaxLength(50);
        builder.Property(b => b.Language).HasMaxLength(50);
        builder.Property(b => b.CoverImagePath).HasMaxLength(500);
        builder.Property(b => b.Status).HasConversion<string>();

        builder.HasOne(b => b.Publisher)
               .WithMany(p => p.Books)
               .HasForeignKey(b => b.PublisherId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
