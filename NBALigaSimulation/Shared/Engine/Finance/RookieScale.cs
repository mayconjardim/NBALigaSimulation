namespace NBALigaSimulation.Shared.Engine.Finance;

public static class RookieScale
{
    public const int FirstRoundContractYears = 4;

    public const int SecondRoundContractYears = 2;
    private const double SecondRoundPercentMin = 0.8;
    private const double SecondRoundPercentMax = 1.2;

    private static readonly double[] FirstRoundPercentByPick = BuildFirstRoundScale();

    private static double[] BuildFirstRoundScale()
    {
        // Pontos de referência: (pick, % cap)
        var points = new[] { (1, 6.9), (5, 4.6), (10, 3.0), (20, 1.8), (30, 1.4) };
        var scale = new double[31]; // índice 0 não usado, 1–30 usados
        for (int i = 0; i < points.Length - 1; i++)
        {
            int p0 = points[i].Item1;
            int p1 = points[i + 1].Item1;
            double v0 = points[i].Item2;
            double v1 = points[i + 1].Item2;
            for (int pick = p0; pick <= p1; pick++)
            {
                double t = (pick - p0) / (double)(p1 - p0);
                scale[pick] = v0 + t * (v1 - v0);
            }
        }
        return scale;
    }

    public static double GetFirstRoundPercentOfCap(int pick)
    {
        if (pick < 1 || pick > 30) return 0;
        return FirstRoundPercentByPick[pick] / 100.0;
    }

    public static int GetFirstYearSalary(int pick, int salaryCap = SalaryCapConstants.SalaryCap)
    {
        return (int)(salaryCap * GetFirstRoundPercentOfCap(pick));
    }

    public static int GetRookieSalary(int pick, int yearOfContract, int salaryCap = SalaryCapConstants.SalaryCap)
    {
        if (yearOfContract < 1 || yearOfContract > FirstRoundContractYears)
            return 0;
        return GetFirstYearSalary(pick, salaryCap);
    }

    public static int GetRookieContractTotal(int pick, int salaryCap = SalaryCapConstants.SalaryCap)
    {
        int year1 = GetFirstYearSalary(pick, salaryCap);
        return year1 * FirstRoundContractYears;
    }

    public static double GetSecondRoundPercentOfCap(int pick, int firstPickSecondRound = 31, int lastPickSecondRound = 60)
    {
        if (pick < firstPickSecondRound || pick > lastPickSecondRound) return SecondRoundPercentMin / 100.0;
        double t = (pick - firstPickSecondRound) / (double)(lastPickSecondRound - firstPickSecondRound);
        double pct = SecondRoundPercentMax - t * (SecondRoundPercentMax - SecondRoundPercentMin);
        return pct / 100.0;
    }

    public static int GetSecondRoundSalary(int pick, int salaryCap = SalaryCapConstants.SalaryCap)
    {
        return (int)(salaryCap * GetSecondRoundPercentOfCap(pick));
    }
}
