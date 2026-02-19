using NBALigaSimulation.Shared.Engine.Utils;
using NBALigaSimulation.Shared.Models.Games;
using NBALigaSimulation.Shared.Models.Teams;

namespace NBALigaSimulation.Shared.Engine.GameSim.InjuryManager;

/// <summary>
/// Verifica lesões durante a posse e atribui tipo/duração na entidade Player.
/// Refatorado a partir do core.gameSim.injuries do BBGM.
/// </summary>
public static class InjuryActions
{
    /// <summary>Probabilidade por jogador em quadra, por posse: ~0,25 lesões/jogo, 10 jogadores, ~200 posses → 0,25/10/200.</summary>
    private const double InjuryProbabilityPerPossession = 0.25 / 10.0 / 200.0;

    /// <summary>
    /// Escolhe tipo e duração da lesão (depende de healthRank, gastos em saúde e aleatoriedade).
    /// </summary>
    /// <param name="healthRank">1–30: 1 = maior gasto em saúde do time na temporada, 30 = menor.</param>
    /// <returns>Tipo da lesão e jogos restantes (para persistir em Player.InjuryType e Player.InjuryGamesRemaining).</returns>
    public static (string Type, int GamesRemaining) PickInjury(int healthRank)
    {
        double rand = RandomUtils.RandomUniform(0, 10882);
        int i = 0;
        for (; i < InjuriesData.CumSum.Length; i++)
        {
            if (InjuriesData.CumSum[i] >= rand)
                break;
        }
        i = Math.Min(i, InjuriesData.CumSum.Length - 1);

        double healthFactor = (healthRank - 1) / 29.0 + 0.5;
        double gamesRemaining = healthFactor * RandomUtils.RandomUniform(0.25, 1.75) * InjuriesData.GamesRemainings[i];
        int gamesRemainingRounded = Math.Max(1, (int)Math.Round(gamesRemaining));

        return (InjuriesData.Types[i], gamesRemainingRounded);
    }

    /// <summary>
    /// Verifica se houve lesão nesta posse (apenas jogadores em quadra) e aplica na entidade Player.
    /// Não determina o tipo de lesão aqui; apenas decide se o jogador falta o resto do jogo e atribui tipo/duração.
    /// Se houver nova lesão, deve-se chamar UpdatePlayersOnCourt em seguida para substituir o jogador.
    /// </summary>
    /// <param name="game">Jogo em simulação.</param>
    /// <param name="teams">Home (0) e Away (1).</param>
    /// <param name="playersOnCourt">Índices dos jogadores em quadra por time (0 e 1).</param>
    /// <param name="homeHealthRank">Rank de saúde do time da casa (1–30). Se null, usa 15.</param>
    /// <param name="awayHealthRank">Rank de saúde do time visitante (1–30). Se null, usa 15.</param>
    /// <returns>True se houve pelo menos uma nova lesão (para forçar substituição).</returns>
    public static bool CheckAndApplyInjuries(
        Game game,
        Team[] teams,
        int[][] playersOnCourt,
        int? homeHealthRank = null,
        int? awayHealthRank = null)
    {
        bool newInjury = false;
        int homeRank = homeHealthRank ?? 15;
        int awayRank = awayHealthRank ?? 15;

        for (int t = 0; t < 2; t++)
        {
            int healthRank = t == 0 ? homeRank : awayRank;
            for (int p = 0; p < teams[t].Players.Count; p++)
            {
                if (Array.IndexOf(playersOnCourt[t], p) < 0)
                    continue;

                if (RandomUtils.RandomUniform(0, 1) >= InjuryProbabilityPerPossession)
                    continue;

                var player = teams[t].Players[p];
                var (type, gamesRemaining) = PickInjury(healthRank);
                player.InjuryType = type;
                player.InjuryGamesRemaining = gamesRemaining;
                newInjury = true;
            }
        }

        return newInjury;
    }
}
