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

        public static List<Game> Generate1stRoundGames(List<Playoffs> playoffs, Season season)
        {

            List<Game> games = new List<Game>();

            DateTime dataInicial = DateTime.UtcNow.AddDays(1);
            TimeZoneInfo tzBrasilia = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
            dataInicial = new DateTime(dataInicial.Year, dataInicial.Month, dataInicial.Day, 23, 0, 0);
            dataInicial = TimeZoneInfo.ConvertTimeFromUtc(dataInicial, tzBrasilia);


            foreach (var serie in playoffs)
            {

                Game game1 = new Game
                {
                    Type = 1,
                    HomeTeamId = serie.TeamOneId,
                    AwayTeamId = serie.TeamTwoId,
                    GameDate = dataInicial,
                    Season = season,
                };

                games.Add(game1);

                Game game2 = new Game
                {
                    Type = 1,
                    HomeTeamId = serie.TeamOneId,
                    AwayTeamId = serie.TeamTwoId,
                    GameDate = dataInicial.AddMinutes(10),
                    Season = season,
                };

                games.Add(game2);

                Game game3 = new Game
                {
                    Type = 1,
                    HomeTeamId = serie.TeamTwoId,
                    AwayTeamId = serie.TeamOneId,
                    GameDate = dataInicial.AddMinutes(20),
                    Season = season,
                };

                games.Add(game3);

                Game game4 = new Game
                {
                    Type = 1,
                    HomeTeamId = serie.TeamTwoId,
                    AwayTeamId = serie.TeamOneId,
                    GameDate = dataInicial.AddMinutes(30),
                    Season = season,
                };

                games.Add(game4);


                Game game5 = new Game
                {
                    Type = 1,
                    HomeTeamId = serie.TeamOneId,
                    AwayTeamId = serie.TeamTwoId,
                    GameDate = dataInicial.AddMinutes(40),
                    Season = season,
                };

                games.Add(game5);

                Game game6 = new Game
                {
                    Type = 1,
                    HomeTeamId = serie.TeamTwoId,
                    AwayTeamId = serie.TeamOneId,
                    GameDate = dataInicial.AddMinutes(50),
                    Season = season,
                };

                games.Add(game6);

                Game game7 = new Game
                {
                    Type = 1,
                    HomeTeamId = serie.TeamOneId,
                    AwayTeamId = serie.TeamTwoId,
                    GameDate = dataInicial.AddMinutes(60),
                    Season = season,
                };

                games.Add(game7);

            }

            return games;
        }

    }
}
