using LibraryMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryMS.Infrastructure.Data.Configurations;

public class ActivityLogConfiguration : IEntityTypeConfiguration<ActivityLog>
{
    public void Configure(EntityTypeBuilder<ActivityLog> builder)
    {
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Action).IsRequired().HasMaxLength(100);
        builder.Property(l => l.EntityType).IsRequired().HasMaxLength(100);
        builder.Property(l => l.EntityId).HasMaxLength(50);
    }
}
