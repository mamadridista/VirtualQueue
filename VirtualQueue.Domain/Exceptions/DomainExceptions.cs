namespace VirtualQueue.Domain.Exceptions;

/// <summary>
/// Base type for exceptions raised when a domain invariant is violated
/// (e.g., an illegal state transition). Kept distinct from infrastructure
/// or validation exceptions so the API layer can map it to a specific
/// HTTP status code (typically 409 Conflict or 400 Bad Request).
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}

/// <summary>
/// Raised when a QueueEntry is asked to move into a status that isn't
/// reachable from its current status (e.g., Completed -> Waiting).
/// </summary>
public sealed class InvalidQueueStateTransitionException : DomainException
{
    public InvalidQueueStateTransitionException(string currentStatus, string attemptedStatus)
        : base($"Cannot transition queue entry from '{currentStatus}' to '{attemptedStatus}'.")
    {
    }
}
