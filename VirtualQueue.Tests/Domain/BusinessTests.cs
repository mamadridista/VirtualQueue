using FluentAssertions;
using VirtualQueue.Domain.Entities;
using Xunit;

namespace VirtualQueue.Tests.Domain;

public class BusinessTests
{
    [Fact]
    public void Constructor_WithValidData_CreatesBusiness()
    {
        var ownerId = Guid.NewGuid();

        var business = new Business("Sam's Barber Shop", ownerId);

        business.Name.Should().Be("Sam's Barber Shop");
        business.OwnerUserId.Should().Be(ownerId);
        business.Id.Should().NotBeEmpty();
        business.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_WithInvalidName_ThrowsArgumentException(string? invalidName)
    {
        var act = () => new Business(invalidName!, Guid.NewGuid());

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Rename_WithValidName_UpdatesName()
    {
        var business = new Business("Old Name", Guid.NewGuid());

        business.Rename("New Name");

        business.Name.Should().Be("New Name");
    }

    [Fact]
    public void Rename_WithBlankName_ThrowsArgumentException()
    {
        var business = new Business("Old Name", Guid.NewGuid());

        var act = () => business.Rename("   ");

        act.Should().Throw<ArgumentException>();
    }
}
