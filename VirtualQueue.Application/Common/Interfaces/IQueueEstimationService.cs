using VirtualQueue.Domain.Entities;

namespace VirtualQueue.Application.Common.Interfaces;

/// <summary>
/// Calculates a customer's queue position and estimated wait time.
/// Deliberately takes an in-memory collection of QueueEntry rather than
/// querying the database itself - this keeps the formula pure and trivial
/// to unit test (no DbContext, no mocking framework needed, just plain
/// objects in / a result out).
/// </summary>
public interface IQueueEstimationService
{
    QueueEstimationResult Estimate(IReadOnlyCollection<QueueEntry> queueEntriesForService, int averageServiceDurationMinutes);
}

/// <param name="PeopleAhead">Count of entries still Waiting.</param>
/// <param name="EstimatedWaitMinutes">PeopleAhead * average service duration.</param>
public sealed record QueueEstimationResult(int PeopleAhead, int EstimatedWaitMinutes);
