using NBALigaSimulation.Shared.Models;

namespace NBALigaSimulation.Shared.Engine
{
    public static class RecordHelper
    {

        public static void RecordStatHelperTeam(int t, int p, string s, int Id, Team[] teams, int season, int amt = 1)
        {

            var team = teams[t];
            if (team != null)
            {
                if (team.Stats == null)
                {
                    team.Stats = new List<TeamGameStats>();
                }

                var lastStats = team.Stats.Find(s => s.GameId == Id);

                if (lastStats == null || lastStats.GameId != Id)
                {
                    lastStats = new TeamGameStats { GameId = Id, Season = season };
                    team.Stats.Add(lastStats);
                }

                if (s == "Fg")
                {
                    lastStats.Fg += amt;
                }
                else if (s == "Fga")
                {
                    lastStats.Fga += amt;
                }
                else if (s == "FgAtRim")
                {
                    lastStats.FgAtRim += amt;
                }
                else if (s == "FgaAtRim")
                {
                    lastStats.FgaAtRim += amt;
                }
                else if (s == "FgLowPost")
                {
                    lastStats.FgLowPost += amt;
                }
                else if (s == "FgaLowPost")
                {
                    lastStats.FgaLowPost += amt;
                }
                else if (s == "FgMidRange")
                {
                    lastStats.FgMidRange += amt;
                }
                else if (s == "FgaMidRange")
                {
                    lastStats.FgaMidRange += amt;
                }
                else if (s == "Tp")
                {
                    lastStats.Tp += amt;
                }
                else if (s == "Tpa")
                {
                    lastStats.Tpa += amt;
                }
                else if (s == "Ft")
                {
                    lastStats.Ft += amt;
                }
                else if (s == "Fta")
                {
                    lastStats.Fta += amt;
                }
                else if (s == "Orb")
                {
                    lastStats.Orb += amt;
                }
                else if (s == "Drb")
                {
                    lastStats.Drb += amt;
                }
                else if (s == "Ast")
                {
                    lastStats.Ast += amt;
                }
                else if (s == "Tov")
                {
                    lastStats.Tov += amt;
                }
                else if (s == "Stl")
                {
                    lastStats.Stl += amt;
                }
                else if (s == "Blk")
                {
                    lastStats.Blk += amt;
                }
                else if (s == "Pf")
                {
                    lastStats.Pf += amt;
                }
                else if (s == "Pts")
                {
                    lastStats.Pts += amt;
                }
                else if (s == "Trb")
                {
                    lastStats.Trb += amt;
                }
            }
        }

        public static void RecordStatHelperPlayer(DateTime gameDate, int t, int p, string s, int Id, Team[] teams, int season, int amt = 1, double amntDouble = 1.0)
        {
            int opp = 0;
            if (t == 0)
            {
                opp = 1;
            }

            var player = teams[t].Players.Find(player => player.RosterOrder == p);
            if (player != null)
            {
                if (player.Stats == null)
                {
                    player.Stats = new List<PlayerGameStats>();
                }

                var lastStats = player.Stats.Find(s => s.GameId == Id);

                if (lastStats == null || lastStats.GameId != Id)
                {
                    lastStats = new PlayerGameStats { GameId = Id, TeamId = teams[t].Id, Name = player.Name, Season = season, OppAbrev = teams[opp].Abrv, GameDate = gameDate };
                    player.Stats.Add(lastStats);
                }

                var regularStats = player.RegularStats.Find(s => s.Season == season);

                if (regularStats == null)
                {
                    regularStats = new PlayerRegularStats { TeamId = teams[t].Id, Name = player.Name, Season = season, TeamAbrv = teams[t].Abrv };
                    player.RegularStats.Add(regularStats);
                }

                lastStats.OppAbrev = teams[opp].Abrv;
                lastStats.GameDate = gameDate;

                if (s == "Fg")
                {
                    lastStats.Fg += amt;
                    regularStats.Fg += amt;

                }
                else if (s == "Fga")
                {
                    lastStats.Fga += amt;
                    regularStats.Fga += amt;
                }
                else if (s == "FgAtRim")
                {
                    lastStats.FgAtRim += amt;
                    regularStats.FgAtRim += amt;
                }
                else if (s == "FgaAtRim")
                {
                    lastStats.FgaAtRim += amt;
                    regularStats.FgaAtRim += amt;
                }
                else if (s == "FgLowPost")
                {
                    lastStats.FgLowPost += amt;
                    regularStats.FgLowPost += amt;
                }
                else if (s == "FgaLowPost")
                {
                    lastStats.FgaLowPost += amt;
                    regularStats.FgaLowPost += amt;
                }
                else if (s == "FgMidRange")
                {
                    lastStats.FgMidRange += amt;
                    regularStats.FgMidRange += amt;
                }
                else if (s == "FgaMidRange")
                {
                    lastStats.FgaMidRange += amt;
                    regularStats.FgaMidRange += amt;
                }
                else if (s == "Tp")
                {
                    lastStats.Tp += amt;
                    regularStats.Tp += amt;
                }
                else if (s == "Tpa")
                {
                    lastStats.Tpa += amt;
                    regularStats.Tpa += amt;
                }
                else if (s == "Ft")
                {
                    lastStats.Ft += amt;
                    regularStats.Ft += amt;
                }
                else if (s == "Fta")
                {
                    lastStats.Fta += amt;
                    regularStats.Fta += amt;
                }
                else if (s == "Orb")
                {
                    lastStats.Orb += amt;
                    regularStats.Orb += amt;
                }
                else if (s == "Drb")
                {
                    lastStats.Drb += amt;
                    regularStats.Drb += amt;
                }
                else if (s == "Ast")
                {
                    lastStats.Ast += amt;
                    regularStats.Ast += amt;
                }
                else if (s == "Tov")
                {
                    lastStats.Tov += amt;
                    regularStats.Tov += amt;
                }
                else if (s == "Stl")
                {
                    lastStats.Stl += amt;
                    regularStats.Stl += amt;
                }
                else if (s == "Blk")
                {
                    lastStats.Blk += amt;
                    regularStats.Blk += amt;
                }
                else if (s == "Pf")
                {
                    lastStats.Pf += amt;
                    regularStats.Pf += amt;
                }
                else if (s == "Pts")
                {
                    lastStats.Pts += amt;
                    regularStats.Pts += amt;
                }
                else if (s == "Trb")
                {
                    lastStats.Trb += amt;
                    regularStats.Trb += amt;
                }
                else if (s == "Gs")
                {
                    lastStats.Gs += amt;
                    regularStats.Gs += amt;
                }
                else if (s == "CourtTime")
                {
                    lastStats.CourtTime += amntDouble;
                }
                else if (s == "BenchTime")
                {
                    lastStats.BenchTime += amntDouble;
                }
                else if (s == "Energy")
                {
                    lastStats.Energy += amntDouble;
                }
                else if (s == "Min")
                {
                    lastStats.Min += amntDouble;
                    regularStats.Min += amntDouble;
                }
            }
        }

        public static void RecordStatHelperPlayerMinutes(int t, int p, string s, int Id, Team[] teams, double amt = 1.0)
        {
            var player = teams[t].Players.Find(player => player.RosterOrder == p);
            var stats = player.Stats.Find(stats => stats.GameId == Id);

            if (player != null && stats != null)
            {
                switch (s)
                {
                    case "CourtTime":
                        stats.CourtTime += amt;
                        break;
                    case "BenchTime":
                        stats.BenchTime += amt;
                        break;
                    case "Energy":
                        stats.Energy += amt;
                        break;
                    case "Min":
                        stats.Min += amt;
                        break;
                }
            }
        }

    }
}
