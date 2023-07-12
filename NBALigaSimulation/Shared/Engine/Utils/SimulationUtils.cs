using NBALigaSimulation.Shared.Dtos;
using NBALigaSimulation.Shared.Models;

namespace NBALigaSimulation.Shared.Engine.Utils
{
    public static class SimulationUtils
    {


        public static void UpdateTeamStats(Game game)
        {

            int season = game.Season.Year;

            Team homeTeam = game.HomeTeam;
            Team awayTeam = game.AwayTeam;

            TeamGameStats HomeGameStat = game.TeamGameStats.Find(g => g.TeamId == homeTeam.Id);
            TeamGameStats AwayGameStat = game.TeamGameStats.Find(g => g.TeamId == awayTeam.Id);

            if (game.Type == 0)
            {
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
            else if (game.Type == 1)
            {
                TeamPlayoffsStats HomeGame = homeTeam.TeamPlayoffsStats.Find(t => t.Season == season);
                TeamPlayoffsStats AwayGame = awayTeam.TeamPlayoffsStats.Find(t => t.Season == season);

                if (HomeGame == null)
                {
                    HomeGame = new TeamPlayoffsStats
                    {
                        Season = season
                    };
                    homeTeam.TeamPlayoffsStats.Add(HomeGame);
                }

                if (AwayGame == null)
                {
                    AwayGame = new TeamPlayoffsStats
                    {
                        Season = season
                    };
                    awayTeam.TeamPlayoffsStats.Add(AwayGame);
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
        }

        public static void UpdatePlayerGames(Game game)
        {
            Team homeTeam = game.HomeTeam;
            Team awayTeam = game.AwayTeam;
            int season = game.Season.Year;

            if (game.Type == 0)
            {

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
            else if (game.Type == 1)
            {

                foreach (var player in homeTeam.Players)
                {
                    var playoffStats = player.PlayoffsStats.Find(s => s.Season == season);
                    if (playoffStats != null && player.Stats.Find(p => p.GameId == game.Id).Min > 0)
                    {
                        playoffStats.Games += 1;
                    }
                }

                foreach (var player in awayTeam.Players)
                {
                    var playoffStats = player.PlayoffsStats.Find(s => s.Season == season);
                    if (playoffStats != null && player.Stats.Find(p => p.GameId == game.Id).Min > 0)
                    {
                        playoffStats.Games += 1;
                    }
                }

            }
        }
        public static bool ArePlayersInCorrectOrder(List<Player> Players)
        {
            for (int i = 0; i < Players.Count; i++)
            {
                if (Players[i].RosterOrder != i)
                {
                    return false;
                }
            }

            return true;
        }

        public static void AdjustRosterOrder(List<Player> Players)
        {
            for (int i = 0; i < Players.Count; i++)
            {
                Players[i].RosterOrder = i;
            }
        }

        public static void UpdateStandings(List<Team> teams, int season)
        {
            List<TeamRegularStats> eastTeams = teams.Where(t => t.Conference == "East")
                                       .Select(t => t.TeamRegularStats.Find(trs => trs.Season == season))
                                       .OrderByDescending(trs => trs.WinPct)
                                       .ToList();

            List<TeamRegularStats> westTeams = teams.Where(t => t.Conference == "West")
                                       .Select(t => t.TeamRegularStats.Find(trs => trs.Season == season))
                                       .OrderByDescending(trs => trs.WinPct)
                                       .ToList();

            foreach (var team in eastTeams)
            {
                int index = eastTeams.IndexOf(team);
                team.ConfRank = index + 1;
            }

            foreach (var team in westTeams)
            {
                int index = westTeams.IndexOf(team);
                team.ConfRank = index + 1;
            }

        }

    }
}
