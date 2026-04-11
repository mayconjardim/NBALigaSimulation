using NBALigaSimulation.Shared.Engine.Finance;
using Xunit;

namespace NBALigaSimulation.Engine.Tests.Finance;

public class SalaryCapConstantsTests
{
    [Theory]
    [InlineData(0, 0.25)]
    [InlineData(6, 0.25)]
    [InlineData(7, 0.30)]
    [InlineData(10, 0.35)]
    public void GetMaxSalaryPercentByExperience_ReturnsExpectedTier(int yearsExperience, double expected)
    {
        Assert.Equal(expected, SalaryCapConstants.GetMaxSalaryPercentByExperience(yearsExperience));
    }

    [Theory]
    [InlineData(0, 0.008)]
    [InlineData(2, 0.015)]
    [InlineData(3, 0.015)]
    [InlineData(10, 0.0215)]
    public void GetMinSalaryPercentByExperience_ReturnsExpectedTier(int yearsExperience, double expected)
    {
        Assert.Equal(expected, SalaryCapConstants.GetMinSalaryPercentByExperience(yearsExperience));
    }

    [Fact]
    public void GetMaxContractYears_ReturnsExpectedValues()
    {
        Assert.Equal(5, SalaryCapConstants.GetMaxContractYears(true));
        Assert.Equal(4, SalaryCapConstants.GetMaxContractYears(false));
    }

    [Fact]
    public void GetSalaryHelpers_UseConfiguredSalaryCap()
    {
        Assert.Equal(25_000_000, SalaryCapConstants.GetMaxSalaryForFreeAgent(1));
        Assert.Equal(35_000_000, SalaryCapConstants.GetMaxSalaryForFreeAgent(10));
        Assert.Equal(800_000, SalaryCapConstants.GetMinSalaryForFreeAgent(1));
        Assert.Equal(2_150_000, SalaryCapConstants.GetMinSalaryForFreeAgent(10));
    }
}
