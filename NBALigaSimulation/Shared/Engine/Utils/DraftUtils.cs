using NBALigaSimulation.Shared.Engine.Finance;
using NBALigaSimulation.Shared.Models;
using NBALigaSimulation.Shared.Models.Drafts;
using NBALigaSimulation.Shared.Models.Players;
using NBALigaSimulation.Shared.Models.Teams;

namespace NBALigaSimulation.Shared.Engine.Utils
{
    public static class DraftUtils
    {

        /// <summary>Sortea a ordem dos 4 primeiros picks entre os 4 times que não foram aos playoffs (piores 4).</summary>
        public static List<TeamRegularStats> RunLottery(List<TeamRegularStats> teams)
        {
            if (teams.Count < 4)
                throw new InvalidOperationException("A loteria precisa de 4 times.");

            Dictionary<int, int> LOTTO_BALLS = new Dictionary<int, int>
            {
                { 0, 250 },
                { 1, 199 },
                { 2, 156 },
                { 3, 119 },
            };

            List<TeamRegularStats> order = new List<TeamRegularStats>();
            var random = new Random();
            while (LOTTO_BALLS.Count > 0)
            {
                double N = LOTTO_BALLS.Values.Sum();
                int draw = weightedRandomChoice(LOTTO_BALLS.Keys.ToList(), LOTTO_BALLS.Values.ToList(), N);
                order.Add(teams[draw]);
                LOTTO_BALLS.Remove(draw);
            }

            return order;
        }

        private static int weightedRandomChoice(List<int> choices, List<int> weights, double totalWeight)
        {
            Random random = new Random();
            double randomNumber = random.NextDouble() * totalWeight;
            double weightSum = 0;
            for (int i = 0; i < choices.Count; i++)
            {
                weightSum += weights[i];
                if (randomNumber < weightSum)
                {
                    return choices[i];
                }
            }
            return choices.Last();
        }

        public static List<Draft> GenerateDraft(DraftLottery lottery, int season, List<TeamRegularStats> regularStats, List<TeamDraftPicks> picks, List<Team> teams)
        {

            // Apenas os 4 da loteria são excluídos; o restante entra na ordem por campanha (piores primeiro)
            List<TeamRegularStats> newStatsList = regularStats
                .OrderBy(stats => stats.WinPct)
                .ThenBy(stats => stats.Points)
                .Where(stats => stats.TeamId != lottery.FirstTeamId
                             && stats.TeamId != lottery.SecondTeamId
                             && stats.TeamId != lottery.ThirdTeamId
                             && stats.TeamId != lottery.FourthTeamId)
                .ToList();

            var firstTeamPick = picks.FirstOrDefault(p => p.Original == lottery.FirstTeam);
            var secondTeamPick = picks.FirstOrDefault(p => p.Original == lottery.SecondTeam);
            var thirdTeamPick = picks.FirstOrDefault(p => p.Original == lottery.ThirdTeam);
            var fourthTeamPick = picks.FirstOrDefault(p => p.Original == lottery.FourthTeam);

            int pick1TeamId = firstTeamPick?.TeamId ?? lottery.FirstTeamId;
            int pick2TeamId = secondTeamPick?.TeamId ?? lottery.SecondTeamId;
            int pick3TeamId = thirdTeamPick?.TeamId ?? lottery.ThirdTeamId;
            int pick4TeamId = fourthTeamPick?.TeamId ?? lottery.FourthTeamId;

            var sortedStats = regularStats
                .OrderByDescending(stats => stats.WinPct)
                .ThenBy(stats => stats.Points)
                .ToList();

            List<Draft> newDraft = new List<Draft>
            {
                new Draft { Original = lottery.FirstTeam, Pick = 1, Season = season, TeamId = pick1TeamId, Round = 1, DateTime = null, Player = null, PlayerId = null },
                new Draft { Original = lottery.SecondTeam, Pick = 2, Season = season, TeamId = pick2TeamId, Round = 1, DateTime = null, Player = null, PlayerId = null },
                new Draft { Original = lottery.ThirdTeam, Pick = 3, Season = season, TeamId = pick3TeamId, Round = 1, DateTime = null, Player = null, PlayerId = null },
                new Draft { Original = lottery.FourthTeam, Pick = 4, Season = season, TeamId = pick4TeamId, Round = 1, DateTime = null, Player = null, PlayerId = null },
            };

            int pickNumber = 5;
            int countRound1 = Math.Min(16, newStatsList.Count);

            for (int i = 0; i < countRound1; i++)
            {
                var stat = newStatsList[i];
                string teamAbrv = stat.Team?.Abrv ?? "";
                int? teamId = picks
                    .Where(pick => pick.Original == teamAbrv && pick.Round == 1)
                    .Select(pick => (int?)pick.TeamId)
                    .FirstOrDefault();

                newDraft.Add(new Draft
                {
                    Original = teamAbrv,
                    Pick = pickNumber++,
                    Season = season,
                    TeamId = teamId ?? stat.TeamId,
                    Round = 1,
                    DateTime = null,
                    Player = null,
                    PlayerId = null,
                });
            }

            int pickNumberRound2 = pickNumber;
            int countRound2 = Math.Min(22, sortedStats.Count);

            for (int i = 0; i < countRound2; i++)
            {
                var stat = sortedStats[i];
                string teamAbrv = stat.Team?.Abrv ?? "";
                int? teamId = picks
                    .Where(pick => pick.Original == teamAbrv && pick.Round == 2)
                    .Select(pick => (int?)pick.TeamId)
                    .FirstOrDefault();

                newDraft.Add(new Draft
                {
                    Original = teamAbrv,
                    Pick = pickNumberRound2++,
                    Season = season,
                    TeamId = teamId ?? stat.TeamId,
                    Round = 2,
                    DateTime = null,
                    Player = null,
                    PlayerId = null,
                });
            }



            return newDraft;
        }

        public static PlayerContract RookieContracts(int pick, int season, int round = 1)
        {
            var playerContract = new PlayerContract();
            if (round == 2)
            {
                playerContract.Amount = RookieScale.GetSecondRoundSalary(pick);
                playerContract.Exp = season + RookieScale.SecondRoundContractYears;
            }
            else
            {
                playerContract.Amount = RookieScale.GetFirstYearSalary(pick);
                playerContract.Exp = season + RookieScale.FirstRoundContractYears;
            }
            return playerContract;
        }



    }
}
