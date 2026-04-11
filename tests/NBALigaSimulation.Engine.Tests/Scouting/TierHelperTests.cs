using NBALigaSimulation.Shared.Engine.Scouting;
using Xunit;

namespace NBALigaSimulation.Engine.Tests.Scouting;

public class TierHelperTests
{
    [Theory]
    [InlineData(0, Tier.VeryWeak)]
    [InlineData(34, Tier.VeryWeak)]
    [InlineData(35, Tier.BelowAverage)]
    [InlineData(49, Tier.BelowAverage)]
    [InlineData(50, Tier.Average)]
    [InlineData(59, Tier.Average)]
    [InlineData(60, Tier.Good)]
    [InlineData(74, Tier.Good)]
    [InlineData(75, Tier.Elite)]
    [InlineData(100, Tier.Elite)]
    public void GetTier_ReturnsExpectedTier(int value, Tier expected)
    {
        Assert.Equal(expected, TierHelper.GetTier(value));
    }
}
