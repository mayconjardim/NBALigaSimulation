using NBALigaSimulation.Shared.Models;

namespace NBALigaSimulation.Shared.Engine.Utils
{
    public static class PlayoffsUtils
    {


        public static List<Playoffs> Generate1stRound(List<Team> east, List<Team> west, int season)
        {

            List<Playoffs> playoffs = new List<Playoffs>();

            Playoffs Series1 = new Playoffs
            {
                Season = season,
                SeriesId = 1,
                Complete = false,
                TeamOneId = east.FirstOrDefault(t => t.TeamRegularStats.Any(trs => trs.Season == season && trs.ConfRank == 1))?.Id ?? 0,
                TeamTwoId = east.FirstOrDefault(t => t.TeamRegularStats.Any(trs => trs.Season == season && trs.ConfRank == 8))?.Id ?? 0,
                WinsTeamOne = 0,
                WinsTeamTwo = 0
            };

            playoffs.Add(Series1);

            Playoffs Series2 = new Playoffs
            {
                Season = season,
                SeriesId = 2,
                Complete = false,
                TeamOneId = east.FirstOrDefault(t => t.TeamRegularStats.Any(trs => trs.Season == season && trs.ConfRank == 4))?.Id ?? 0,
                TeamTwoId = east.FirstOrDefault(t => t.TeamRegularStats.Any(trs => trs.Season == season && trs.ConfRank == 5))?.Id ?? 0,
                WinsTeamOne = 0,
                WinsTeamTwo = 0
            };

            playoffs.Add(Series2);

            Playoffs Series3 = new Playoffs
            {
                Season = season,
                SeriesId = 3,
                Complete = false,
                TeamOneId = east.FirstOrDefault(t => t.TeamRegularStats.Any(trs => trs.Season == season && trs.ConfRank == 3))?.Id ?? 0,
                TeamTwoId = east.FirstOrDefault(t => t.TeamRegularStats.Any(trs => trs.Season == season && trs.ConfRank == 6))?.Id ?? 0,
                WinsTeamOne = 0,
                WinsTeamTwo = 0
            };

            playoffs.Add(Series3);

            Playoffs Series4 = new Playoffs
            {
                Season = season,
                SeriesId = 4,
                Complete = false,
                TeamOneId = east.FirstOrDefault(t => t.TeamRegularStats.Any(trs => trs.Season == season && trs.ConfRank == 2))?.Id ?? 0,
                TeamTwoId = east.FirstOrDefault(t => t.TeamRegularStats.Any(trs => trs.Season == season && trs.ConfRank == 7))?.Id ?? 0,
                WinsTeamOne = 0,
                WinsTeamTwo = 0
            };

            playoffs.Add(Series4);

            Playoffs Series5 = new Playoffs
            {
                Season = season,
                SeriesId = 5,
                Complete = false,
                TeamOneId = west.FirstOrDefault(t => t.TeamRegularStats.Any(trs => trs.Season == season && trs.ConfRank == 1))?.Id ?? 0,
                TeamTwoId = west.FirstOrDefault(t => t.TeamRegularStats.Any(trs => trs.Season == season && trs.ConfRank == 8))?.Id ?? 0,
                WinsTeamOne = 0,
                WinsTeamTwo = 0
            };

            playoffs.Add(Series5);

            Playoffs Series6 = new Playoffs
            {
                Season = season,
                SeriesId = 6,
                Complete = false,
                TeamOneId = west.FirstOrDefault(t => t.TeamRegularStats.Any(trs => trs.Season == season && trs.ConfRank == 4))?.Id ?? 0,
                TeamTwoId = west.FirstOrDefault(t => t.TeamRegularStats.Any(trs => trs.Season == season && trs.ConfRank == 5))?.Id ?? 0,
                WinsTeamOne = 0,
                WinsTeamTwo = 0
            };

            playoffs.Add(Series6);

            Playoffs Series7 = new Playoffs
            {
                Season = season,
                SeriesId = 7,
                Complete = false,
                TeamOneId = west.FirstOrDefault(t => t.TeamRegularStats.Any(trs => trs.Season == season && trs.ConfRank == 3))?.Id ?? 0,
                TeamTwoId = west.FirstOrDefault(t => t.TeamRegularStats.Any(trs => trs.Season == season && trs.ConfRank == 6))?.Id ?? 0,
                WinsTeamOne = 0,
                WinsTeamTwo = 0
            };

            playoffs.Add(Series7);


            Playoffs Series8 = new Playoffs
            {
                Season = season,
                SeriesId = 8,
                Complete = false,
                TeamOneId = west.FirstOrDefault(t => t.TeamRegularStats.Any(trs => trs.Season == season && trs.ConfRank == 2))?.Id ?? 0,
                TeamTwoId = west.FirstOrDefault(t => t.TeamRegularStats.Any(trs => trs.Season == season && trs.ConfRank == 7))?.Id ?? 0,
                WinsTeamOne = 0,
                WinsTeamTwo = 0
            };

            playoffs.Add(Series8);

            return playoffs;

        }

    }
}
