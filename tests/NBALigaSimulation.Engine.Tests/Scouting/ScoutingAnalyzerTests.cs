using NBALigaSimulation.Shared.Engine.Scouting;
using NBALigaSimulation.Shared.Models.Players;
using Xunit;

namespace NBALigaSimulation.Engine.Tests.Scouting;

public class ScoutingAnalyzerTests
{
    [Theory]
    [InlineData("GF", "SG")]
    [InlineData("FC", "C")]
    [InlineData("G", "SG")]
    [InlineData("F", "SF")]
    [InlineData("PG", "PG")]
    public void NormalizePositionForProfile_ReturnsExpectedBaseProfile(string input, string expected)
    {
        Assert.Equal(expected, ScoutingAnalyzer.NormalizePositionForProfile(input));
    }

    [Fact]
    public void GetAnalysisData_FlagsWeakCoreAttributes_AndStrongBonusAttributes()
    {
        var ratings = CreateRatings();
        ratings.Pss = 20;
        ratings.Drb = 30;
        ratings.Oiq = 55;
        ratings.Spd = 65;
        ratings.Tp = 85;
        ratings.Diq = 76;
        ratings.Reb = 80;

        var (limitations, strengths) = ScoutingAnalyzer.GetAnalysisData(ratings, "PG");

        Assert.Contains(limitations, item => item.Attr == "Pss" && item.Tier == Tier.VeryWeak);
        Assert.Contains(limitations, item => item.Attr == "Drb" && item.Tier == Tier.VeryWeak);
        Assert.Contains(strengths, item => item.Attr == "Tp" && item.Tier == Tier.Elite);
        Assert.Contains(strengths, item => item.Attr == "Diq" && item.Tier == Tier.Elite);
        Assert.Contains(strengths, item => item.Attr == "Reb" && item.Tier == Tier.Elite);
    }

    [Fact]
    public void OrderForReport_SortsStrengthsBeforeAverageAndWeaknesses()
    {
        var items = new List<ScoutingAnalyzer.AnalysisItem>
        {
            new("Pss", Tier.BelowAverage),
            new("Tp", Tier.Elite),
            new("Oiq", Tier.Average),
            new("Diq", Tier.Good),
            new("Drb", Tier.VeryWeak)
        };

        var ordered = ScoutingAnalyzer.OrderForReport(items).ToList();

        Assert.Equal(Tier.Elite, ordered[0].Tier);
        Assert.Equal(Tier.Good, ordered[1].Tier);
        Assert.Equal(Tier.Average, ordered[2].Tier);
        Assert.Equal(Tier.BelowAverage, ordered[3].Tier);
        Assert.Equal(Tier.VeryWeak, ordered[4].Tier);
    }

    private static PlayerRatings CreateRatings()
    {
        return new PlayerRatings
        {
            Season = 2026,
            Pss = 50,
            Drb = 50,
            Oiq = 50,
            Spd = 50,
            Ins = 50,
            Reb = 50,
            Stre = 50,
            Hgt = 50,
            Tp = 50,
            Diq = 50,
            Dnk = 50,
            Endu = 50,
            Ft = 50,
            Jmp = 50,
            Pot = 50,
            Fg = 50
        };
    }
}
