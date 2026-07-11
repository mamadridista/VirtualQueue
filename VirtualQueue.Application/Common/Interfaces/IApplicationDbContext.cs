using Microsoft.EntityFrameworkCore;
using VirtualQueue.Domain.Entities;

namespace VirtualQueue.Application.Common.Interfaces;

/// <summary>
/// Abstraction over ApplicationDbContext. The Api layer depends on this
/// interface (registered in DI), never on the concrete EF Core class in
/// Infrastructure. This is the Dependency Inversion half of SOLID at work:
/// swapping EF Core for another provider later would only touch Infrastructure.
/// </summary>
public interface IApplicationDbContext
{
    DbSet<Business> Businesses { get; }
    DbSet<Service> Services { get; }
    DbSet<QueueEntry> QueueEntries { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
