using NBALigaSimulation.Shared.Engine.Finance;
using Xunit;

namespace NBALigaSimulation.Engine.Tests.Finance;

public class RookieScaleTests
{
    [Fact]
    public void GetFirstRoundPercentOfCap_ReturnsZero_ForInvalidPick()
    {
        Assert.Equal(0, RookieScale.GetFirstRoundPercentOfCap(0));
        Assert.Equal(0, RookieScale.GetFirstRoundPercentOfCap(31));
    }

    [Fact]
    public void GetFirstRoundPercentOfCap_DecreasesAcrossRound()
    {
        double firstPick = RookieScale.GetFirstRoundPercentOfCap(1);
        double tenthPick = RookieScale.GetFirstRoundPercentOfCap(10);
        double thirtiethPick = RookieScale.GetFirstRoundPercentOfCap(30);

        Assert.True(firstPick > tenthPick);
        Assert.True(tenthPick > thirtiethPick);
    }

    [Fact]
    public void GetRookieContractTotal_EqualsFourTimesFirstYearSalary()
    {
        int firstYearSalary = RookieScale.GetFirstYearSalary(7);
        int contractTotal = RookieScale.GetRookieContractTotal(7);

        Assert.Equal(firstYearSalary * RookieScale.FirstRoundContractYears, contractTotal);
    }

    [Fact]
    public void GetSecondRoundPercentOfCap_StaysWithinExpectedRange()
    {
        double earlySecondRound = RookieScale.GetSecondRoundPercentOfCap(31);
        double lateSecondRound = RookieScale.GetSecondRoundPercentOfCap(60);

        Assert.True(earlySecondRound > lateSecondRound);
        Assert.InRange(earlySecondRound, 0.008, 0.012);
        Assert.InRange(lateSecondRound, 0.008, 0.012);
    }
}
