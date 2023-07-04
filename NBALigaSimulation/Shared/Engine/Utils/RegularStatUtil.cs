using NBALigaSimulation.Shared.Models;
using System.Security.Cryptography.X509Certificates;

namespace NBALigaSimulation.Shared.Engine.Utils
{
    public static class RegularStatUtil
    {


        public static void UpdateTeamRegularStats(Game game)
        {

            int season = game.Season.Year;

            Team homeTeam = game.HomeTeam;
            Team awayTeam = game.AwayTeam;

            TeamGameStats HomeGameStat = game.TeamGameStats.Find(g => g.TeamId == homeTeam.Id);
            TeamGameStats AwayGameStat = game.TeamGameStats.Find(g => g.TeamId == awayTeam.Id);


            TeamRegularStats HomeGame = homeTeam.TeamRegularStats.Find(t => t.Season == season);
            TeamRegularStats AwayGame = awayTeam.TeamRegularStats.Find(t => t.Season == season);

            if (HomeGame == null)
            {
                HomeGame = new TeamRegularStats
                {
                    Season = season
                };
                homeTeam.TeamRegularStats.Add(HomeGame);
            }

            if (AwayGame == null)
            {
                AwayGame = new TeamRegularStats
                {
                    Season = season
                };
                awayTeam.TeamRegularStats.Add(AwayGame);
            }

            if (HomeGameStat.Pts > AwayGameStat.Pts)
            {
                HomeGame.HomeWins += 1;
                AwayGame.RoadLosses += 1;
            }
            else
            {
                HomeGame.HomeLosses += 1;
                AwayGame.RoadWins += 1;

            }

            //HomeTeam
            HomeGame.Points += HomeGameStat.Pts;
            HomeGame.AllowedPoints += AwayGameStat.Pts;

            HomeGame.Steals += HomeGameStat.Stl;
            HomeGame.AllowedStealS += AwayGameStat.Stl;

            HomeGame.Rebounds += HomeGameStat.Trb;
            HomeGame.AllowedRebounds += AwayGameStat.Trb;

            HomeGame.Assists += HomeGameStat.Ast;
            HomeGame.AllowedAssists += AwayGameStat.Ast;

            HomeGame.Blocks += HomeGameStat.Blk;
            HomeGame.AllowedBlocks += AwayGameStat.Blk;

            HomeGame.Turnovers += HomeGameStat.Tov;
            HomeGame.AllowedTurnovers += AwayGameStat.Tov;

            HomeGame.FGA += HomeGameStat.Fga;
            HomeGame.FGM += HomeGameStat.Fg;
            HomeGame.AllowedFGA += AwayGameStat.Fga;
            HomeGame.AllowedFGM += AwayGameStat.Fg;

            HomeGame.TPA += HomeGameStat.Tpa;
            HomeGame.TPM += HomeGameStat.Tp;
            HomeGame.Allowed3PA += AwayGameStat.Tpa;
            HomeGame.Allowed3PM += AwayGameStat.Tp;

            HomeGame.FTM += HomeGameStat.Ft;
            HomeGame.FTA += HomeGameStat.Fta;
            HomeGame.AllowedFTM += AwayGameStat.Ft;
            HomeGame.AllowedFTA += AwayGameStat.Fta;

            //AwayTeam
            AwayGame.Points += AwayGameStat.Pts;
            AwayGame.AllowedPoints += HomeGameStat.Pts;

            AwayGame.Steals += AwayGameStat.Stl;
            AwayGame.AllowedStealS += HomeGameStat.Stl;

            AwayGame.Rebounds += AwayGameStat.Trb;
            AwayGame.AllowedRebounds += HomeGameStat.Trb;

            AwayGame.Assists += AwayGameStat.Ast;
            AwayGame.AllowedAssists += HomeGameStat.Ast;

            AwayGame.Blocks += AwayGameStat.Blk;
            AwayGame.AllowedBlocks += HomeGameStat.Blk;

            AwayGame.Turnovers += AwayGameStat.Tov;
            AwayGame.AllowedTurnovers += HomeGameStat.Tov;

            AwayGame.FGA += AwayGameStat.Fga;
            AwayGame.FGM += AwayGameStat.Fg;
            AwayGame.AllowedFGA += HomeGameStat.Fga;
            AwayGame.AllowedFGM += HomeGameStat.Fg;

            AwayGame.TPA += AwayGameStat.Tpa;
            AwayGame.TPM += AwayGameStat.Tp;
            AwayGame.Allowed3PA += HomeGameStat.Tpa;
            AwayGame.Allowed3PM += HomeGameStat.Tp;

            AwayGame.FTM += AwayGameStat.Ft;
            AwayGame.FTA += AwayGameStat.Fta;
            AwayGame.AllowedFTM += HomeGameStat.Ft;
            AwayGame.AllowedFTA += HomeGameStat.Fta;
        }

        public static void UpdatePlayerGames(Game game)
        {
            Team homeTeam = game.HomeTeam;
            Team awayTeam = game.AwayTeam;
            int season = game.Season.Year;

            foreach (var player in homeTeam.Players)
            {
                var regularStats = player.RegularStats.Find(s => s.Season == season);
                if (regularStats != null && player.Stats.Find(p => p.GameId == game.Id).Min > 0)
                {
                    regularStats.Games += 1;
                }
            }

            foreach (var player in awayTeam.Players)
            {
                var regularStats = player.RegularStats.Find(s => s.Season == season);
                if (regularStats != null && player.Stats.Find(p => p.GameId == game.Id).Min > 0)
                {
                    regularStats.Games += 1;
                }
            }
        }

    }
}
