using VirtualQueue.Domain.Common;

namespace VirtualQueue.Domain.Entities;

/// <summary>
/// A registered local business (salon, clinic, repair shop, etc.).
/// Owns a collection of Services, each of which runs its own queue.
/// </summary>
public class Business : BaseEntity
{
    public string Name { get; private set; } = null!;

    /// <summary>
    /// Identifier of the authenticated owner account. Kept as a plain Guid
    /// rather than a navigation property to a User entity, since user/auth
    /// management is out of scope for Sprint 1 and likely lives in a
    /// separate identity provider or Identity table later.
    /// </summary>
    public Guid OwnerUserId { get; private set; }

    public DateTime CreatedAt { get; private set; }

    private readonly List<Service> _services = new();
    public IReadOnlyCollection<Service> Services => _services.AsReadOnly();

    // EF Core requires a parameterless constructor to materialize entities.
    // Kept private so it can't be misused from application code.
    private Business() { }

    public Business(string name, Guid ownerUserId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Business name is required.", nameof(name));

        Name = name.Trim();
        OwnerUserId = ownerUserId;
        CreatedAt = DateTime.UtcNow;
    }

    public void Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Business name is required.", nameof(newName));

        Name = newName.Trim();
    }
}
