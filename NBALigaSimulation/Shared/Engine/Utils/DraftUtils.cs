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

        public static List<Draft> GenerateDraft(DraftLottery lottery, int season, List<TeamRegularStats> regularStats, List<TeamDraftPicks> picks)
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

            picks.RemoveAll(p => p.Original == lottery.FirstTeam);
            picks.RemoveAll(p => p.Original == lottery.SecondTeam);
            picks.RemoveAll(p => p.Original == lottery.ThirdTeam);
            picks.RemoveAll(p => p.Original == lottery.FourthTeam);
            picks.RemoveAll(p => p.Original == lottery.FifthTeam);
            picks.RemoveAll(p => p.Original == lottery.SixthTeam);

            var sortedStats = regularStats.OrderByDescending(stats => stats.WinPct).ToList();

            newDraft.Add(new Draft
            {
                Original = lottery.FirstTeam,
                Pick = 1,
                Season = season,
                TeamId = firstTeamPick.Id
            });

            newDraft.Add(new Draft
            {
                Original = lottery.FirstTeam,
                Pick = 2,
                Season = season,
                TeamId = SecondTeamPick.Id
            });

            newDraft.Add(new Draft
            {
                Original = lottery.FirstTeam,
                Pick = 3,
                Season = season,
                TeamId = ThirdTeamPick.Id
            });

            newDraft.Add(new Draft
            {
                Original = lottery.FirstTeam,
                Pick = 4,
                Season = season,
                TeamId = FourthTeamPick.Id
            });

            newDraft.Add(new Draft
            {
                Original = lottery.FirstTeam,
                Pick = 5,
                Season = season,
                TeamId = FifthTeamPick.Id
            });

            newDraft.Add(new Draft
            {
                Original = lottery.FirstTeam,
                Pick = 6,
                Season = season,
                TeamId = SixthTeamPick.Id
            });

            newDraft.Add(new Draft
            {
                Original = sortedStats[0].Team.Abrv,
                Pick = 7,
                Season = season,
                TeamId = picks.Where(pick => pick.Original == sortedStats[0].Team.Abrv && pick.Round == 1).Select(pick => pick.Id).FirstOrDefault()
            });

            newDraft.Add(new Draft
            {
                Original = sortedStats[1].Team.Abrv,
                Pick = 8,
                Season = season,
                TeamId = picks.Where(pick => pick.Original == sortedStats[1].Team.Abrv && pick.Round == 1).Select(pick => pick.Id).FirstOrDefault()
            });

            newDraft.Add(new Draft
            {
                Original = sortedStats[2].Team.Abrv,
                Pick = 9,
                Season = season,
                TeamId = picks.Where(pick => pick.Original == sortedStats[2].Team.Abrv && pick.Round == 1).Select(pick => pick.Id).FirstOrDefault()
            });

             newDraft.Add(new Draft
            {
                Original = sortedStats[3].Team.Abrv,
                Pick = 10,
                Season = season,
                TeamId = picks.Where(pick => pick.Original == sortedStats[3].Team.Abrv && pick.Round == 1).Select(pick => pick.Id).FirstOrDefault()
            });

               newDraft.Add(new Draft
            {
                Original = sortedStats[4].Team.Abrv,
                Pick = 11,
                Season = season,
                TeamId = picks.Where(pick => pick.Original == sortedStats[4].Team.Abrv && pick.Round == 1).Select(pick => pick.Id).FirstOrDefault()
            });

            newDraft.Add(new Draft
            {
                Original = sortedStats[5].Team.Abrv,
                Pick = 12,
                Season = season,
                TeamId = picks.Where(pick => pick.Original == sortedStats[5].Team.Abrv && pick.Round == 1).Select(pick => pick.Id).FirstOrDefault()
            });

            newDraft.Add(new Draft
            {
                Original = sortedStats[6].Team.Abrv,
                Pick = 13,
                Season = season,
                TeamId = picks.Where(pick => pick.Original == sortedStats[6].Team.Abrv && pick.Round == 1).Select(pick => pick.Id).FirstOrDefault()
            });

             newDraft.Add(new Draft
            {
                Original = sortedStats[7].Team.Abrv,
                Pick = 14,
                Season = season,
                TeamId = picks.Where(pick => pick.Original == sortedStats[7].Team.Abrv && pick.Round == 1).Select(pick => pick.Id).FirstOrDefault()
            });

               newDraft.Add(new Draft
            {
                Original = sortedStats[8].Team.Abrv,
                Pick = 15,
                Season = season,
                TeamId = picks.Where(pick => pick.Original == sortedStats[8].Team.Abrv && pick.Round == 1).Select(pick => pick.Id).FirstOrDefault()
            });

            newDraft.Add(new Draft
            {
                Original = sortedStats[9].Team.Abrv,
                Pick = 16,
                Season = season,
                TeamId = picks.Where(pick => pick.Original == sortedStats[9].Team.Abrv && pick.Round == 1).Select(pick => pick.Id).FirstOrDefault()
            });

            newDraft.Add(new Draft
            {
                Original = sortedStats[10].Team.Abrv,
                Pick = 17,
                Season = season,
                TeamId = picks.Where(pick => pick.Original == sortedStats[10].Team.Abrv && pick.Round == 1).Select(pick => pick.Id).FirstOrDefault()
            });

             newDraft.Add(new Draft
            {
                Original = sortedStats[11].Team.Abrv,
                Pick = 18,
                Season = season,
                TeamId = picks.Where(pick => pick.Original == sortedStats[11].Team.Abrv && pick.Round == 1).Select(pick => pick.Id).FirstOrDefault()
            });

               newDraft.Add(new Draft
            {
                Original = sortedStats[12].Team.Abrv,
                Pick = 19,
                Season = season,
                TeamId = picks.Where(pick => pick.Original == sortedStats[12].Team.Abrv && pick.Round == 1).Select(pick => pick.Id).FirstOrDefault()
            });

            newDraft.Add(new Draft
            {
                Original = sortedStats[13].Team.Abrv,
                Pick = 20,
                Season = season,
                TeamId = picks.Where(pick => pick.Original == sortedStats[13].Team.Abrv && pick.Round == 1).Select(pick => pick.Id).FirstOrDefault()
            });

            newDraft.Add(new Draft
            {
                Original = sortedStats[14].Team.Abrv,
                Pick = 21,
                Season = season,
                TeamId = picks.Where(pick => pick.Original == sortedStats[14].Team.Abrv && pick.Round == 1).Select(pick => pick.Id).FirstOrDefault()
            });

             newDraft.Add(new Draft
            {
                Original = sortedStats[15].Team.Abrv,
                Pick = 22,
                Season = season,
                TeamId = picks.Where(pick => pick.Original == sortedStats[15].Team.Abrv && pick.Round == 1).Select(pick => pick.Id).FirstOrDefault()
            });

            return newDraft;
        }


    }
}
