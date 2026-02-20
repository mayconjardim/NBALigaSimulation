using NBALigaSimulation.Shared.Engine.Finance;
using NBALigaSimulation.Shared.Models.Players;
using NBALigaSimulation.Shared.Models.Seasons;

namespace NBALigaSimulation.Shared.Engine.Utils;

/// <summary>
/// Gera contratos para todos os jogadores da liga, respeitando o salary cap, para testes de trades/FA.
/// </summary>
public static class ContractTestUtils
{
    /// <summary>Alvo de uso do cap por time (90% para deixar espaço para FA/trades).</summary>
    private const double CapUsageTarget = 0.90;

    public static void AssignContractsForTest(List<Player> players, int seasonYear)
    {
        var byTeam = players
            .Where(p => p.TeamId != 21 && p.TeamId != 22 && p.Ratings?.Count > 0)
            .GroupBy(p => p.TeamId)
            .ToList();

        foreach (var teamGroup in byTeam)
        {
            var teamPlayers = teamGroup.OrderByDescending(p => p.Ratings!.Last().CalculateOvr).ToList();
            double totalOvrWeight = 0;
            var proposals = new List<(Player Player, int Amount, int Years)>();

            foreach (var p in teamPlayers)
            {
                var r = p.Ratings!.Last();
                int exp = p.Ratings.Count;
                int maxSal = SalaryCapConstants.GetMaxSalaryForFreeAgent(exp);
                int minSal = SalaryCapConstants.GetMinSalaryForFreeAgent(exp);

                double ovrNorm = Math.Clamp(r.CalculateOvr / 100.0, 0, 1);
                int amount = minSal + (int)(Math.Pow(ovrNorm, 1.2) * (maxSal - minSal));
                amount = Math.Max(minSal, Math.Min(maxSal, amount));
                amount = 100_000 * (int)Math.Round((double)amount / 100_000);

                int years = r.CalculateOvr >= 75 ? 5 : r.CalculateOvr >= 65 ? 4 : r.CalculateOvr >= 55 ? 3 : r.CalculateOvr >= 45 ? 2 : 1;
                years = Math.Min(years, exp >= 7 ? 5 : 4);

                proposals.Add((p, amount, years));
                totalOvrWeight += amount;
            }

            if (proposals.Count == 0) continue;

            int targetTotal = (int)(SalaryCapConstants.SalaryCap * CapUsageTarget);
            if (totalOvrWeight > 0 && totalOvrWeight > targetTotal)
            {
                double scale = targetTotal / totalOvrWeight;
                proposals = proposals.Select(x =>
                {
                    int scaled = (int)(x.Amount * scale);
                    int min = SalaryCapConstants.GetMinSalaryForFreeAgent(x.Player.Ratings!.Count);
                    scaled = Math.Max(min, scaled);
                    scaled = 100_000 * (int)Math.Round((double)scaled / 100_000);
                    return (x.Player, scaled, x.Years);
                }).ToList();
            }

            foreach (var (player, amount, years) in proposals)
            {
                int exp = player.Ratings!.Count;
                int expiration = seasonYear + years - 1;
                if (player.Contract != null)
                {
                    player.Contract.Amount = amount;
                    player.Contract.Exp = expiration;
                }
                else
                {
                    player.Contract = new PlayerContract
                    {
                        PlayerId = player.Id,
                        Amount = amount,
                        Exp = expiration
                    };
                }
            }
        }
    }
}
