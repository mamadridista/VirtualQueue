using FluentAssertions;
using VirtualQueue.Application.Services;
using VirtualQueue.Domain.Entities;
using Xunit;

namespace VirtualQueue.Tests.Application;

public class QueueEstimationServiceTests
{
    private readonly QueueEstimationService _sut = new();

    [Fact]
    public void Estimate_WithThreePeopleWaiting_ReturnsCorrectPositionAndWait()
    {
        var serviceId = Guid.NewGuid();
        var entries = new List<QueueEntry>
        {
            new(serviceId, "+15550000001"),
            new(serviceId, "+15550000002"),
            new(serviceId, "+15550000003"),
        };

        var result = _sut.Estimate(entries, averageServiceDurationMinutes: 25);

        // Mirrors the exact example from the product brief:
        // "3 people ahead of you, your turn in about 25 minutes" for a
        // 25-min average would actually be 75 - here we verify the raw
        // formula (peopleAhead * avgDuration), matching a 3x25=75 minute wait.
        result.PeopleAhead.Should().Be(3);
        result.EstimatedWaitMinutes.Should().Be(75);
    }

    [Fact]
    public void Estimate_WithEmptyQueue_ReturnsZeroWait()
    {
        var result = _sut.Estimate(Array.Empty<QueueEntry>(), averageServiceDurationMinutes: 20);

        result.PeopleAhead.Should().Be(0);
        result.EstimatedWaitMinutes.Should().Be(0);
    }

    [Fact]
    public void Estimate_IgnoresNonWaitingEntries()
    {
        var serviceId = Guid.NewGuid();
        var waitingEntry = new QueueEntry(serviceId, "+15550000001");

        var beingServedEntry = new QueueEntry(serviceId, "+15550000002");
        beingServedEntry.MarkAsServing();

        var completedEntry = new QueueEntry(serviceId, "+15550000003");
        completedEntry.MarkAsServing();
        completedEntry.MarkAsCompleted();

        var entries = new List<QueueEntry> { waitingEntry, beingServedEntry, completedEntry };

        var result = _sut.Estimate(entries, averageServiceDurationMinutes: 10);

        // Only the Waiting entry should count - defensive filtering means
        // a caller passing an unfiltered list still gets a correct answer.
        result.PeopleAhead.Should().Be(1);
        result.EstimatedWaitMinutes.Should().Be(10);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Estimate_WithNonPositiveAverageDuration_ThrowsArgumentOutOfRangeException(int invalidDuration)
    {
        var act = () => _sut.Estimate(Array.Empty<QueueEntry>(), invalidDuration);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
