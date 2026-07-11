using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VirtualQueue.Domain.Entities;

namespace VirtualQueue.Infrastructure.Persistence.Configurations;

public class QueueEntryConfiguration : IEntityTypeConfiguration<QueueEntry>
{
    public void Configure(EntityTypeBuilder<QueueEntry> builder)
    {
        builder.ToTable("QueueEntries");

        builder.HasKey(q => q.Id);

        builder.Property(q => q.CustomerMobileNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(q => q.Status)
            .IsRequired()
            .HasConversion<string>()   // store enum as readable text, not a magic int, for easier ad-hoc DB inspection/reporting
            .HasMaxLength(20);

        builder.Property(q => q.JoinedAt)
            .IsRequired();

        builder.Property(q => q.ServiceId)
            .IsRequired();

        // This is the hottest query in the whole system: "everyone Waiting
        // for this Service, ordered by join time" is run every time a
        // customer checks their position and every time Call Next fires.
        builder.HasIndex(q => new { q.ServiceId, q.Status, q.JoinedAt });

        // Supports reporting queries grouped by day (Phase covering LINQ GroupBy).
        builder.HasIndex(q => q.CompletedAt);
    }
}
