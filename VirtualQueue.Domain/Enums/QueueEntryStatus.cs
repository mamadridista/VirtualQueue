namespace VirtualQueue.Domain.Enums;

/// <summary>
/// Lifecycle of a single QueueEntry.
///
/// Waiting    -> customer has joined, waiting to be called
/// Serving    -> business owner clicked "Call Next", customer is being served now
/// Completed  -> service finished normally
/// Cancelled  -> customer left the queue before being served (Level 2+ feature,
///               defined now so the schema doesn't need to change later)
/// </summary>
public enum QueueEntryStatus
{
    Waiting = 0,
    Serving = 1,
    Completed = 2,
    Cancelled = 3
}
