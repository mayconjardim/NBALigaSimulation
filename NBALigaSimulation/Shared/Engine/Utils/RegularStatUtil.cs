using NBALigaSimulation.Shared.Models;

namespace NBALigaSimulation.Shared.Engine.Utils
{
    public static class RegularStatUtil
    {


        public static void TeamStatHelper(Game game)
        {

            Team HomeTeam = game.HomeTeam;
            Team AwayTeam = game.AwayTeam;

            TeamGameStats HomeGameStat = game.TeamGameStats.Find(g => g.TeamId == HomeTeam.Id);
            TeamGameStats AwayGameStat = game.TeamGameStats.Find(g => g.TeamId == AwayTeam.Id);

            if (HomeTeam.TeamRegularStats == null)
            {
                HomeTeam.TeamRegularStats = new TeamRegularStats();
            }

            if (AwayTeam.TeamRegularStats == null)
            {
                AwayTeam.TeamRegularStats = new TeamRegularStats();
            }

            HomeTeam.TeamRegularStats.Season = game.Season.Year;
            HomeTeam.TeamRegularStats.Season = game.Season.Year;

            if (HomeGameStat.Pts > AwayGameStat.Pts)
            {
                HomeTeam.TeamRegularStats.HomeWins += 1;
                AwayTeam.TeamRegularStats.RoadLosses += 1;
            }
            else
            {
                HomeTeam.TeamRegularStats.HomeLosses += 1;
                AwayTeam.TeamRegularStats.RoadWins += 1;

            }

            //HomeTeam
            HomeTeam.TeamRegularStats.Points += HomeGameStat.Pts;
            HomeTeam.TeamRegularStats.AllowedPoints += AwayGameStat.Pts;

            HomeTeam.TeamRegularStats.Steals += HomeGameStat.Stl;
            HomeTeam.TeamRegularStats.AllowedStealS += AwayGameStat.Stl;

            HomeTeam.TeamRegularStats.Rebounds += HomeGameStat.Trb;
            HomeTeam.TeamRegularStats.AllowedRebounds += AwayGameStat.Trb;

            HomeTeam.TeamRegularStats.Assists += HomeGameStat.Ast;
            HomeTeam.TeamRegularStats.AllowedAssists += AwayGameStat.Ast;

            HomeTeam.TeamRegularStats.Blocks += HomeGameStat.Blk;
            HomeTeam.TeamRegularStats.AllowedBlocks += AwayGameStat.Blk;

            HomeTeam.TeamRegularStats.Turnovers += HomeGameStat.Tov;
            HomeTeam.TeamRegularStats.AllowedTurnovers += AwayGameStat.Tov;

            HomeTeam.TeamRegularStats.FGA += HomeGameStat.Fga;
            HomeTeam.TeamRegularStats.FGM += HomeGameStat.Fg;
            HomeTeam.TeamRegularStats.AllowedFGA += AwayGameStat.Fga;
            HomeTeam.TeamRegularStats.AllowedFGM += AwayGameStat.Fg;

            HomeTeam.TeamRegularStats.TPA += HomeGameStat.Tpa;
            HomeTeam.TeamRegularStats.TPM += HomeGameStat.Tp;
            HomeTeam.TeamRegularStats.Allowed3PA += AwayGameStat.Tpa;
            HomeTeam.TeamRegularStats.Allowed3PM += AwayGameStat.Tp;

            HomeTeam.TeamRegularStats.FTM += HomeGameStat.Ft;
            HomeTeam.TeamRegularStats.FTA += HomeGameStat.Fta;
            HomeTeam.TeamRegularStats.AllowedFTM += AwayGameStat.Ft;
            HomeTeam.TeamRegularStats.AllowedFTA += AwayGameStat.Fta;

            //AwayTeam
            AwayTeam.TeamRegularStats.Points += AwayGameStat.Pts;
            AwayTeam.TeamRegularStats.AllowedPoints += HomeGameStat.Pts;

            AwayTeam.TeamRegularStats.Steals += AwayGameStat.Stl;
            AwayTeam.TeamRegularStats.AllowedStealS += HomeGameStat.Stl;

            AwayTeam.TeamRegularStats.Rebounds += AwayGameStat.Trb;
            AwayTeam.TeamRegularStats.AllowedRebounds += HomeGameStat.Trb;

            AwayTeam.TeamRegularStats.Assists += AwayGameStat.Ast;
            AwayTeam.TeamRegularStats.AllowedAssists += HomeGameStat.Ast;

            AwayTeam.TeamRegularStats.Blocks += AwayGameStat.Blk;
            AwayTeam.TeamRegularStats.AllowedBlocks += HomeGameStat.Blk;

            AwayTeam.TeamRegularStats.Turnovers += AwayGameStat.Tov;
            AwayTeam.TeamRegularStats.AllowedTurnovers += HomeGameStat.Tov;

            AwayTeam.TeamRegularStats.FGA += AwayGameStat.Fga;
            AwayTeam.TeamRegularStats.FGM += AwayGameStat.Fg;
            AwayTeam.TeamRegularStats.AllowedFGA += HomeGameStat.Fga;
            AwayTeam.TeamRegularStats.AllowedFGM += HomeGameStat.Fg;

            AwayTeam.TeamRegularStats.TPA += AwayGameStat.Tpa;
            AwayTeam.TeamRegularStats.TPM += AwayGameStat.Tp;
            AwayTeam.TeamRegularStats.Allowed3PA += HomeGameStat.Tpa;
            AwayTeam.TeamRegularStats.Allowed3PM += HomeGameStat.Tp;

            AwayTeam.TeamRegularStats.FTM += AwayGameStat.Ft;
            AwayTeam.TeamRegularStats.FTA += AwayGameStat.Fta;
            AwayTeam.TeamRegularStats.AllowedFTM += HomeGameStat.Ft;
            AwayTeam.TeamRegularStats.AllowedFTA += HomeGameStat.Fta;

        }


    }
}
