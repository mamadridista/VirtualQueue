using Microsoft.EntityFrameworkCore;
using VirtualQueue.Application.Common.Interfaces;
using VirtualQueue.Domain.Entities;

namespace VirtualQueue.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Business> Businesses => Set<Business>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<QueueEntry> QueueEntries => Set<QueueEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Scans this assembly for every IEntityTypeConfiguration<T> and applies
        // it automatically, instead of listing each one by hand here. Keeps
        // OnModelCreating from growing unbounded as entities are added.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
