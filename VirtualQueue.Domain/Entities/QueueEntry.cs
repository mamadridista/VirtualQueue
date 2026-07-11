using VirtualQueue.Domain.Enums;
using VirtualQueue.Domain.Exceptions;
using VirtualQueue.Domain.Common;

namespace VirtualQueue.Domain.Entities;

/// <summary>
/// A single customer's position in a Service's queue.
///
/// State transitions are enforced here (not in a service/controller) so the
/// rule "you can't complete an entry that was never called" can never be
/// bypassed regardless of which layer calls into this entity. This is the
/// "avoid anemic design" requirement from the sprint brief.
/// </summary>
public class QueueEntry : BaseEntity
{
    public Guid ServiceId { get; private set; }
    public Service? Service { get; private set; }

    public string CustomerMobileNumber { get; private set; } = null!;

    public QueueEntryStatus Status { get; private set; }

    public DateTime JoinedAt { get; private set; }
    public DateTime? CalledAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    private QueueEntry() { }

    public QueueEntry(Guid serviceId, string customerMobileNumber)
    {
        if (serviceId == Guid.Empty)
            throw new ArgumentException("A queue entry must belong to a valid service.", nameof(serviceId));

        if (string.IsNullOrWhiteSpace(customerMobileNumber))
            throw new ArgumentException("Customer mobile number is required.", nameof(customerMobileNumber));

        ServiceId = serviceId;
        CustomerMobileNumber = customerMobileNumber.Trim();
        Status = QueueEntryStatus.Waiting;
        JoinedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Called when the business owner clicks "Call Next" and this entry
    /// is the one being pulled from the queue.
    /// </summary>
    public void MarkAsServing()
    {
        if (Status != QueueEntryStatus.Waiting)
            throw new InvalidQueueStateTransitionException(Status.ToString(), QueueEntryStatus.Serving.ToString());

        Status = QueueEntryStatus.Serving;
        CalledAt = DateTime.UtcNow;
    }

    public void MarkAsCompleted()
    {
        if (Status != QueueEntryStatus.Serving)
            throw new InvalidQueueStateTransitionException(Status.ToString(), QueueEntryStatus.Completed.ToString());

        Status = QueueEntryStatus.Completed;
        CompletedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Customer leaves the queue voluntarily before being served.
    /// Not exposed via an API endpoint until Level 2, but modeled now so
    /// the schema/state machine doesn't need to change later.
    /// </summary>
    public void Cancel()
    {
        if (Status != QueueEntryStatus.Waiting)
            throw new InvalidQueueStateTransitionException(Status.ToString(), QueueEntryStatus.Cancelled.ToString());

        Status = QueueEntryStatus.Cancelled;
    }
}
