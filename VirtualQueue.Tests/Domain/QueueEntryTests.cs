using FluentAssertions;
using VirtualQueue.Domain.Entities;
using VirtualQueue.Domain.Enums;
using VirtualQueue.Domain.Exceptions;
using Xunit;

namespace VirtualQueue.Tests.Domain;

public class QueueEntryTests
{
    [Fact]
    public void Constructor_WithValidData_CreatesEntryInWaitingStatus()
    {
        var serviceId = Guid.NewGuid();

        var entry = new QueueEntry(serviceId, "+15551234567");

        entry.ServiceId.Should().Be(serviceId);
        entry.CustomerMobileNumber.Should().Be("+15551234567");
        entry.Status.Should().Be(QueueEntryStatus.Waiting);
        entry.CalledAt.Should().BeNull();
        entry.CompletedAt.Should().BeNull();
    }

    [Fact]
    public void MarkAsServing_FromWaiting_TransitionsToServingAndSetsCalledAt()
    {
        var entry = new QueueEntry(Guid.NewGuid(), "+15551234567");

        entry.MarkAsServing();

        entry.Status.Should().Be(QueueEntryStatus.Serving);
        entry.CalledAt.Should().NotBeNull();
    }

    [Fact]
    public void MarkAsServing_WhenAlreadyServing_ThrowsInvalidQueueStateTransitionException()
    {
        var entry = new QueueEntry(Guid.NewGuid(), "+15551234567");
        entry.MarkAsServing();

        var act = () => entry.MarkAsServing();

        act.Should().Throw<InvalidQueueStateTransitionException>();
    }

    [Fact]
    public void MarkAsCompleted_FromServing_TransitionsToCompletedAndSetsCompletedAt()
    {
        var entry = new QueueEntry(Guid.NewGuid(), "+15551234567");
        entry.MarkAsServing();

        entry.MarkAsCompleted();

        entry.Status.Should().Be(QueueEntryStatus.Completed);
        entry.CompletedAt.Should().NotBeNull();
    }

    [Fact]
    public void MarkAsCompleted_WithoutBeingCalledFirst_ThrowsInvalidQueueStateTransitionException()
    {
        // This is the exact rule the sprint report calls out:
        // "you can't complete an entry that was never called".
        var entry = new QueueEntry(Guid.NewGuid(), "+15551234567");

        var act = () => entry.MarkAsCompleted();

        act.Should().Throw<InvalidQueueStateTransitionException>()
            .WithMessage("*Waiting*Completed*");
    }

    [Fact]
    public void Cancel_FromWaiting_TransitionsToCancelled()
    {
        var entry = new QueueEntry(Guid.NewGuid(), "+15551234567");

        entry.Cancel();

        entry.Status.Should().Be(QueueEntryStatus.Cancelled);
    }

    [Fact]
    public void Cancel_AfterBeingCalled_ThrowsInvalidQueueStateTransitionException()
    {
        var entry = new QueueEntry(Guid.NewGuid(), "+15551234567");
        entry.MarkAsServing();

        var act = () => entry.Cancel();

        act.Should().Throw<InvalidQueueStateTransitionException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_WithInvalidMobileNumber_ThrowsArgumentException(string? invalidNumber)
    {
        var act = () => new QueueEntry(Guid.NewGuid(), invalidNumber!);

        act.Should().Throw<ArgumentException>();
    }
}
