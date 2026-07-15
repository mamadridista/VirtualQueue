using VirtualQueue.Application.Common.Interfaces;
using VirtualQueue.Domain.Entities;
using VirtualQueue.Domain.Enums;

namespace VirtualQueue.Application.Services;

/// <summary>
/// Level 1 estimation formula: PeopleAhead * AverageServiceDurationMinutes.
/// Intentionally simple (no time-of-day weighting, no per-staff-member
/// parallelism) - the PO scoped that complexity out for this level.
/// Swapping this out later (e.g., for a weighted average based on the last
/// N completed entries) only means providing a different
/// IQueueEstimationService implementation; nothing that calls this
/// interface needs to change.
/// </summary>
public sealed class QueueEstimationService : IQueueEstimationService
{
    public QueueEstimationResult Estimate(IReadOnlyCollection<QueueEntry> queueEntriesForService, int averageServiceDurationMinutes)
    {
        if (averageServiceDurationMinutes <= 0)
            throw new ArgumentOutOfRangeException(nameof(averageServiceDurationMinutes), "Average service duration must be greater than zero.");

        // Defensive filter: callers are expected to pass only relevant
        // entries, but a mixed list (e.g., one that accidentally includes
        // a Completed or Cancelled entry) shouldn't silently corrupt the
        // estimate.
        var peopleAhead = queueEntriesForService.Count(e => e.Status == QueueEntryStatus.Waiting);

        var estimatedWaitMinutes = peopleAhead * averageServiceDurationMinutes;

        return new QueueEstimationResult(peopleAhead, estimatedWaitMinutes);
    }
}
