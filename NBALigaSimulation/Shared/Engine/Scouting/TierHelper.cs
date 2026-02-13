namespace NBALigaSimulation.Shared.Engine.Scouting;

public static class TierHelper
{
    public static Tier GetTier(int value)
    {
        if (value < 35) return Tier.VeryWeak;
        if (value < 50) return Tier.BelowAverage;
        if (value < 60) return Tier.Average;
        if (value < 75) return Tier.Good;
        return Tier.Elite;
    }
}
