using NBALigaSimulation.Shared.Models;
using NBALigaSimulation.Shared.Models.Drafts;
using NBALigaSimulation.Shared.Models.Players;
using NBALigaSimulation.Shared.Models.Teams;

namespace NBALigaSimulation.Shared.Engine.Utils
{
    public static class DraftUtils
    {

        public static List<TeamRegularStats> RunLottery(List<TeamRegularStats> teams)
        {

            Dictionary<int, int> LOTTO_BALLS = new Dictionary<int, int>
            {

            {0, 250},
            {1, 199},
            {2, 156},
            {3, 119},
            {4, 88},
            {5, 63},

            };

            List<TeamRegularStats> order = new List<TeamRegularStats>();

            Random random = new Random();
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

            List<TeamRegularStats> newStatsList = regularStats
                .OrderBy(stats => stats.WinPct)
                .ThenBy(stats => stats.Points)
                .Where(stats => stats.TeamId != lottery.FirstTeamId
                             && stats.TeamId != lottery.SecondTeamId
                             && stats.TeamId != lottery.ThirdTeamId
                             && stats.TeamId != lottery.FourthTeamId
                             && stats.TeamId != lottery.FifthTeamId
                             && stats.TeamId != lottery.SixthTeamId)
             .ToList();

            var firstTeamPick = picks.FirstOrDefault(p => p.Original == lottery.FirstTeam);
            var SecondTeamPick = picks.FirstOrDefault(p => p.Original == lottery.SecondTeam);
            var ThirdTeamPick = picks.FirstOrDefault(p => p.Original == lottery.ThirdTeam);
            var FourthTeamPick = picks.FirstOrDefault(p => p.Original == lottery.FourthTeam);
            var FifthTeamPick = picks.FirstOrDefault(p => p.Original == lottery.FifthTeam);
            var SixthTeamPick = picks.FirstOrDefault(p => p.Original == lottery.SixthTeam);

            var sortedStats = regularStats
                .OrderByDescending(stats => stats.WinPct)
                .ThenBy(stats => stats.Points)
                .ToList();

            List<Draft> newDraft = new List<Draft>
            {
                new Draft
                {
                    Original = lottery.FirstTeam,
                    Pick = 1,
                    Season = season,
                    TeamId = firstTeamPick.TeamId,
                    Round = 1,
                    DateTime = null,
                    Player = null,
                    PlayerId = null,
                },

                new Draft
                {
                    Original = lottery.SecondTeam,
                    Pick = 2,
                    Season = season,
                    TeamId = SecondTeamPick.TeamId,
                    Round = 1,
                    DateTime = null,
                    Player = null,
                    PlayerId = null,
                },

                new Draft
                {
                    Original = lottery.ThirdTeam,
                    Pick = 3,
                    Season = season,
                    TeamId = ThirdTeamPick.TeamId,
                    Round = 1,
                    DateTime = null,
                    Player = null,
                    PlayerId = null,
                },

                new Draft
                {
                    Original = lottery.FourthTeam,
                    Pick = 4,
                    Season = season,
                    TeamId = FourthTeamPick.TeamId,
                    Round = 1,
                    DateTime = null,
                    Player = null,
                    PlayerId = null,

                },

                new Draft
                {
                    Original = lottery.FifthTeam,
                    Pick = 5,
                    Season = season,
                    TeamId = FifthTeamPick.TeamId,
                    Round = 1,
                    DateTime = null,
                    Player = null,
                    PlayerId = null,
                },

                new Draft
                {
                    Original = lottery.SixthTeam,
                    Pick = 6,
                    Season = season,
                    TeamId = SixthTeamPick.TeamId,
                    Round = 1,
                    DateTime = null,
                    Player = null,
                    PlayerId = null,
                },

            };

            int pickNumber = 7;

            for (int i = 0; i < 16; i++)
            {
                string teamAbrv = newStatsList[i].Team.Abrv;
                int? teamId = picks
                    .Where(pick => pick.Original == teamAbrv && pick.Round == 1)
                    .Select(pick => pick.TeamId)
                    .FirstOrDefault();

                newDraft.Add(new Draft
                {
                    Original = teamAbrv,
                    Pick = pickNumber++,
                    Season = season,
                    TeamId = (int)teamId,
                    Round = 1,
                    DateTime = null,
                    Player = null,
                    PlayerId = null,
                });
            }

            int pickNumberRound2 = 23;

            for (int i = 0; i < 22; i++)
            {
                string teamAbrv = sortedStats[i].Team.Abrv;
                int? teamId = picks
                    .Where(pick => pick.Original == teamAbrv && pick.Round == 2)
                    .Select(pick => pick.TeamId)
                    .FirstOrDefault();

                newDraft.Add(new Draft
                {
                    Original = teamAbrv,
                    Pick = pickNumberRound2++,
                    Season = season,
                    TeamId = (int)teamId,
                    Round = 2,
                    DateTime = null,
                    Player = null,
                    PlayerId = null,
                });
            }



            return newDraft;
        }

        public static PlayerContract RookieContracts(int pick, int season)
        {
            PlayerContract playerContract = new PlayerContract();
            Dictionary<int, int> contractAmounts = new Dictionary<int, int>
            {
                { 1, 7000000 }, { 2, 6700000 }, { 3, 6400000 }, { 4, 5100000 }, { 5, 4800000 },
                { 6, 4500000 }, { 7, 4200000 }, { 8, 3900000 }, { 9, 3600000 }, { 10, 3300000 },
                { 11, 3100000 }, { 12, 2900000 }, { 13, 2700000 }, { 14, 2500000 }, { 15, 2300000 },
                { 16, 2100000 }, { 17, 1900000 }, { 18, 1700000 }, { 19, 1500000 }, { 20, 1300000 },
                { 21, 1200000 }, { 22, 1100000 }
            };

            if (contractAmounts.ContainsKey(pick))
            {
                playerContract.Amount = contractAmounts[pick];
                playerContract.Exp = season + 4;
            }
            else
            {
                playerContract.Amount = 900000;
                playerContract.Exp = season + 2;
            }

            return playerContract;
        }



    }
}
