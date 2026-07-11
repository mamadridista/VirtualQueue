using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VirtualQueue.Domain.Entities;

namespace VirtualQueue.Infrastructure.Persistence.Configurations;

public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.ToTable("Services");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(s => s.AverageDurationMinutes)
            .IsRequired();

        builder.Property(s => s.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(s => s.BusinessId)
            .IsRequired();

        // Public queue-join page looks up "active services for this
        // business" constantly - composite index supports that query.
        builder.HasIndex(s => new { s.BusinessId, s.IsActive });

        builder.HasMany(s => s.QueueEntries)
            .WithOne(q => q.Service)
            .HasForeignKey(q => q.ServiceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
