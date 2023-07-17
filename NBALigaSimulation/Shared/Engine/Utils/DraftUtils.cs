using NBALigaSimulation.Shared.Models;
using System;

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
            List<Draft> newDraft = new List<Draft>();

            regularStats.RemoveAll(stats => stats.TeamId == lottery.FirstTeamId);
            regularStats.RemoveAll(stats => stats.TeamId == lottery.SecondTeamId);
            regularStats.RemoveAll(stats => stats.TeamId == lottery.ThirdTeamId);
            regularStats.RemoveAll(stats => stats.TeamId == lottery.FourthTeamId);
            regularStats.RemoveAll(stats => stats.TeamId == lottery.FifthTeamId);
            regularStats.RemoveAll(stats => stats.TeamId == lottery.SixthTeamId);

            var firstTeamPick = picks.FirstOrDefault(p => p.Original == lottery.FirstTeam);
            var SecondTeamPick = picks.FirstOrDefault(p => p.Original == lottery.SecondTeam);
            var ThirdTeamPick = picks.FirstOrDefault(p => p.Original == lottery.ThirdTeam);
            var FourthTeamPick = picks.FirstOrDefault(p => p.Original == lottery.FourthTeam);
            var FifthTeamPick = picks.FirstOrDefault(p => p.Original == lottery.FifthTeam);
            var SixthTeamPick = picks.FirstOrDefault(p => p.Original == lottery.SixthTeam);

            var sortedStats = regularStats.OrderByDescending(stats => stats.WinPct).ToList();


           

            return newDraft;
        }


    }
}
