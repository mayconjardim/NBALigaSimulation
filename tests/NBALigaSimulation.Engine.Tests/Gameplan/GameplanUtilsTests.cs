using NBALigaSimulation.Shared.Engine.Gameplan;
using Xunit;

namespace NBALigaSimulation.Engine.Tests.Gameplan;

public class GameplanUtilsTests
{
    [Theory]
    [InlineData(1, 95.7)]
    [InlineData(2, 98.0)]
    [InlineData(3, 101.6)]
    [InlineData(99, 101.6)]
    public void GameplanPace_ReturnsExpectedValues(int input, double expected)
    {
        Assert.Equal(expected, GameplanUtils.GameplanPace(input));
    }

    [Theory]
    [InlineData(1, -0.2)]
    [InlineData(2, 0.0)]
    [InlineData(3, 0.2)]
    [InlineData(99, 0.2)]
    public void GameplanMotion_ReturnsExpectedValues(int input, double expected)
    {
        Assert.Equal(expected, GameplanUtils.GameplanMotion(input));
    }

    [Theory]
    [InlineData(1, -0.2)]
    [InlineData(2, 0.0)]
    [InlineData(3, 0.2)]
    [InlineData(99, 0.2)]
    public void GameplanFocus_ReturnsExpectedValues(int input, double expected)
    {
        Assert.Equal(expected, GameplanUtils.GameplanFocus(input));
    }

    [Theory]
    [InlineData(1, 0.2)]
    [InlineData(2, 0.2)]
    [InlineData(3, 0.2)]
    [InlineData(99, 0.2)]
    public void GameplanDefense_ReturnsExpectedValues(int input, double expected)
    {
        Assert.Equal(expected, GameplanUtils.GameplanDefense(input));
    }
}
