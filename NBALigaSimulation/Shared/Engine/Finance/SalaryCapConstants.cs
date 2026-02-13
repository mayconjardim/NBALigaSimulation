namespace NBALigaSimulation.Shared.Engine.Finance;

public static class SalaryCapConstants
{
    public const int SalaryCap = 100_000_000;

    public const int MaxContractYearsCurrentTeam = 5;
    public const int MaxContractYearsOtherTeam = 4;

    public static double GetMaxSalaryPercentByExperience(int yearsExperience)
    {
        if (yearsExperience >= 10) return 0.35;
        if (yearsExperience >= 7) return 0.30;
        return 0.25;
    }

    public static int GetMaxSalaryForFreeAgent(int yearsExperience)
    {
        return (int)(SalaryCap * GetMaxSalaryPercentByExperience(yearsExperience));
    }

    public static int GetMaxContractYears(bool isCurrentTeam)
    {
        return isCurrentTeam ? MaxContractYearsCurrentTeam : MaxContractYearsOtherTeam;
    }

    public static double GetMinSalaryPercentByExperience(int yearsExperience)
    {
        if (yearsExperience >= 10) return 0.0215;
        if (yearsExperience >= 2 && yearsExperience <= 3) return 0.015;
        return 0.008;
    }

    public static int GetMinSalaryForFreeAgent(int yearsExperience)
    {
        return (int)(SalaryCap * GetMinSalaryPercentByExperience(yearsExperience));
    }
}
