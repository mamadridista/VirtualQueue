using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VirtualQueue.Domain.Entities;

namespace VirtualQueue.Infrastructure.Persistence.Configurations;

public class BusinessConfiguration : IEntityTypeConfiguration<Business>
{
    public void Configure(EntityTypeBuilder<Business> builder)
    {
        builder.ToTable("Businesses");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(b => b.OwnerUserId)
            .IsRequired();

        builder.Property(b => b.CreatedAt)
            .IsRequired();

        // A business is looked up by owner frequently (owner's dashboard
        // listing "my businesses"), so index it.
        builder.HasIndex(b => b.OwnerUserId);

        builder.HasMany(b => b.Services)
            .WithOne(s => s.Business)
            .HasForeignKey(s => s.BusinessId)
            // Deleting a business deletes its services (and, transitively,
            // their queue entries). Acceptable for Level 1; if historical
            // reporting needs to survive business deletion later, this is
            // the place to switch to Restrict + soft-delete instead.
            .OnDelete(DeleteBehavior.Cascade);
    }
}
