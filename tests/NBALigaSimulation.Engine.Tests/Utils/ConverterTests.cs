using NBALigaSimulation.Shared.Engine.Utils;
using Xunit;

namespace NBALigaSimulation.Engine.Tests.Utils;

public class ConverterTests
{
    [Fact]
    public void Composite_UsesDefaultWeights_WhenWeightsAreNull()
    {
        var ratings = new Dictionary<string, double>
        {
            ["tp"] = 80,
            ["oiq"] = 60
        };

        double result = Converter.Composite(ratings, new List<string> { "tp", "oiq" });

        Assert.Equal(0.7, result, 3);
    }

    [Fact]
    public void Composite_UsesConstantComponent50_Correctly()
    {
        var ratings = new Dictionary<string, double>
        {
            ["spd"] = 80,
            ["diq"] = 70
        };

        double result = Converter.Composite(ratings, new List<string> { "50", "spd", "diq" }, new List<double> { 1, 1, 2 });

        Assert.Equal(0.675, result, 3);
    }

    [Fact]
    public void Composite_UsesOvrComponent_WhenProvided()
    {
        var ratings = new Dictionary<string, double>
        {
            ["ovr"] = 90,
            ["tp"] = 80
        };

        double result = Converter.Composite(ratings, new List<string> { "ovr", "tp" });

        Assert.Equal(0.85, result, 3);
    }

    [Fact]
    public void Composite_Throws_WhenRatingIsMissing()
    {
        var ratings = new Dictionary<string, double>
        {
            ["tp"] = 80
        };

        var ex = Assert.Throws<Exception>(() => Converter.Composite(ratings, new List<string> { "tp", "diq" }));

        Assert.Contains("Undefined value", ex.Message);
    }

    [Fact]
    public void Composite_BoundsResult_BetweenZeroAndOne()
    {
        var ratings = new Dictionary<string, double>
        {
            ["tp"] = 180,
            ["diq"] = -50
        };

        double high = Converter.Composite(ratings, new List<string> { "tp" });
        double low = Converter.Composite(ratings, new List<string> { "diq" });

        Assert.Equal(1, high);
        Assert.Equal(0, low);
    }

    [Fact]
    public void HasSkill_ReturnsTrue_WhenWeightedAverageExceedsThreshold()
    {
        bool result = Converter.hasSkill(new List<int> { 80, 70, 75 }, new List<double> { 1, 1, 1 });

        Assert.True(result);
    }

    [Fact]
    public void HasSkill_ReturnsFalse_WhenWeightedAverageDoesNotReachThreshold()
    {
        bool result = Converter.hasSkill(new List<int> { 50, 55, 60 }, new List<double> { 1, 1, 1 });

        Assert.False(result);
    }
}
