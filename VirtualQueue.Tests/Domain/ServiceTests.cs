using FluentAssertions;
using VirtualQueue.Domain.Entities;
using Xunit;

namespace VirtualQueue.Tests.Domain;

public class ServiceTests
{
    [Fact]
    public void Constructor_WithValidData_CreatesActiveService()
    {
        var businessId = Guid.NewGuid();

        var service = new Service(businessId, "Haircut", 20);

        service.BusinessId.Should().Be(businessId);
        service.Name.Should().Be("Haircut");
        service.AverageDurationMinutes.Should().Be(20);
        service.IsActive.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Constructor_WithNonPositiveDuration_ThrowsArgumentOutOfRangeException(int invalidDuration)
    {
        var act = () => new Service(Guid.NewGuid(), "Haircut", invalidDuration);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Constructor_WithEmptyBusinessId_ThrowsArgumentException()
    {
        var act = () => new Service(Guid.Empty, "Haircut", 20);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Deactivate_SetsIsActiveToFalse()
    {
        var service = new Service(Guid.NewGuid(), "Haircut", 20);

        service.Deactivate();

        service.IsActive.Should().BeFalse();
    }

    [Fact]
    public void UpdateAverageDuration_WithValidValue_UpdatesDuration()
    {
        var service = new Service(Guid.NewGuid(), "Haircut", 20);

        service.UpdateAverageDuration(30);

        service.AverageDurationMinutes.Should().Be(30);
    }
}
