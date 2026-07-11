using VirtualQueue.Domain.Common;

namespace VirtualQueue.Domain.Entities;

/// <summary>
/// A service offered by a Business (e.g., "Haircut", "Teeth Cleaning").
/// Each Service runs its own independent queue of QueueEntry records.
/// </summary>
public class Service : BaseEntity
{
    public Guid BusinessId { get; private set; }
    public Business? Business { get; private set; }

    public string Name { get; private set; } = null!;

    public int AverageDurationMinutes { get; private set; }

    public bool IsActive { get; private set; }

    private readonly List<QueueEntry> _queueEntries = new();
    public IReadOnlyCollection<QueueEntry> QueueEntries => _queueEntries.AsReadOnly();

    private Service() { }

    public Service(Guid businessId, string name, int averageDurationMinutes)
    {
        if (businessId == Guid.Empty)
            throw new ArgumentException("A service must belong to a valid business.", nameof(businessId));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Service name is required.", nameof(name));

        if (averageDurationMinutes <= 0)
            throw new ArgumentOutOfRangeException(nameof(averageDurationMinutes), "Average duration must be greater than zero.");

        BusinessId = businessId;
        Name = name.Trim();
        AverageDurationMinutes = averageDurationMinutes;
        IsActive = true;
    }

    public void Deactivate() => IsActive = false;

    public void Activate() => IsActive = true;

    public void UpdateAverageDuration(int minutes)
    {
        if (minutes <= 0)
            throw new ArgumentOutOfRangeException(nameof(minutes), "Average duration must be greater than zero.");

        AverageDurationMinutes = minutes;
    }
}
