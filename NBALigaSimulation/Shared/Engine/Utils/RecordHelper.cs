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
                    team.Stats.Add(new TeamGameStats { GameId = Id, Season = season });
                }
            }


            if (s == "Fg")
            {
                teams[t].Stats.Find(s => s.GameId == Id).Fg += amt;
            }
            else if (s == "Fga")
            {
                teams[t].Stats.Find(s => s.GameId == Id).Fga += amt;
            }
            else if (s == "FgAtRim")
            {
                teams[t].Stats.Find(s => s.GameId == Id).FgAtRim += amt;
            }
            else if (s == "FgaAtRim")
            {
                teams[t].Stats.Find(s => s.GameId == Id).FgaAtRim += amt;
            }
            else if (s == "FgLowPost")
            {
                teams[t].Stats.Find(s => s.GameId == Id).FgLowPost += amt;
            }
            else if (s == "FgaLowPost")
            {
                teams[t].Stats.Find(s => s.GameId == Id).FgaLowPost += amt;
            }
            else if (s == "FgMidRange")
            {
                teams[t].Stats.Find(s => s.GameId == Id).FgMidRange += amt;
            }
            else if (s == "FgaMidRange")
            {
                teams[t].Stats.Find(s => s.GameId == Id).FgaMidRange += amt;
            }
            else if (s == "Tp")
            {
                teams[t].Stats.Find(s => s.GameId == Id).Tp += amt;
            }
            else if (s == "Tpa")
            {
                teams[t].Stats.Find(s => s.GameId == Id).Tpa += amt;
            }
            else if (s == "Ft")
            {
                teams[t].Stats.Find(s => s.GameId == Id).Ft += amt;
            }
            else if (s == "Fta")
            {
                teams[t].Stats.Find(s => s.GameId == Id).Fta += amt;
            }
            else if (s == "Orb")
            {
                teams[t].Stats.Find(s => s.GameId == Id).Orb += amt;
            }
            else if (s == "Drb")
            {
                teams[t].Stats.Find(s => s.GameId == Id).Drb += amt;
            }
            else if (s == "Ast")
            {
                teams[t].Stats.Find(s => s.GameId == Id).Ast += amt;
            }
            else if (s == "Tov")
            {
                teams[t].Stats.Find(s => s.GameId == Id).Tov += amt;
            }
            else if (s == "Stl")
            {
                teams[t].Stats.Find(s => s.GameId == Id).Stl += amt;
            }
            else if (s == "Blk")
            {
                teams[t].Stats.Find(s => s.GameId == Id).Blk += amt;
            }
            else if (s == "Pf")
            {
                teams[t].Stats.Find(s => s.GameId == Id).Pf += amt;
            }
            else if (s == "Pts")
            {
                teams[t].Stats.Find(s => s.GameId == Id).Pts += amt;
            }
            else if (s == "Trb")
            {
                teams[t].Stats.Find(s => s.GameId == Id).Trb += amt;
            }

        }

        public static void RecordStatHelperPlayer(int t, int p, string s, int Id, Team[] teams, int season, int amt = 1, double amntDouble = 1.0)
        {

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
                    player.Stats.Add(new PlayerGameStats { GameId = Id, TeamId = teams[t].Id, Name = player.Name, Season = season });
                }
            }

            if (s == "Fg")
            {
                teams[t].Players.Find(player => player.RosterOrder == p).Stats.Find(s => s.GameId == Id).Fg += amt;
            }
            else if (s == "Fga")
            {
                teams[t].Players.Find(player => player.RosterOrder == p).Stats.Find(s => s.GameId == Id).Fga += amt;
            }
            else if (s == "FgAtRim")
            {
                teams[t].Players.Find(player => player.RosterOrder == p).Stats.Find(s => s.GameId == Id).FgAtRim += amt;
            }
            else if (s == "FgaAtRim")
            {
                teams[t].Players.Find(player => player.RosterOrder == p).Stats.Find(s => s.GameId == Id).FgaAtRim += amt;
            }
            else if (s == "FgLowPost")
            {
                teams[t].Players.Find(player => player.RosterOrder == p).Stats.Find(s => s.GameId == Id).FgLowPost += amt;
            }
            else if (s == "FgaLowPost")
            {
                teams[t].Players.Find(player => player.RosterOrder == p).Stats.Find(s => s.GameId == Id).FgaLowPost += amt;
            }
            else if (s == "FgMidRange")
            {
                teams[t].Players.Find(player => player.RosterOrder == p).Stats.Find(s => s.GameId == Id).FgMidRange += amt;
            }
            else if (s == "FgaMidRange")
            {
                teams[t].Players.Find(player => player.RosterOrder == p).Stats.Find(s => s.GameId == Id).FgaMidRange += amt;
            }
            else if (s == "Tp")
            {
                teams[t].Players.Find(player => player.RosterOrder == p).Stats.Find(s => s.GameId == Id).Tp += amt;
            }
            else if (s == "Tpa")
            {
                teams[t].Players.Find(player => player.RosterOrder == p).Stats.Find(s => s.GameId == Id).Tpa += amt;
            }
            else if (s == "Ft")
            {
                teams[t].Players.Find(player => player.RosterOrder == p).Stats.Find(s => s.GameId == Id).Ft += amt;
            }
            else if (s == "Fta")
            {
                teams[t].Players.Find(player => player.RosterOrder == p).Stats.Find(s => s.GameId == Id).Fta += amt;
            }
            else if (s == "Orb")
            {
                teams[t].Players.Find(player => player.RosterOrder == p).Stats.Find(s => s.GameId == Id).Orb += amt;
            }
            else if (s == "Drb")
            {
                teams[t].Players.Find(player => player.RosterOrder == p).Stats.Find(s => s.GameId == Id).Drb += amt;
            }
            else if (s == "Ast")
            {
                teams[t].Players.Find(player => player.RosterOrder == p).Stats.Find(s => s.GameId == Id).Ast += amt;
            }
            else if (s == "Tov")
            {
                teams[t].Players.Find(player => player.RosterOrder == p).Stats.Find(s => s.GameId == Id).Tov += amt;
            }
            else if (s == "Stl")
            {
                teams[t].Players.Find(player => player.RosterOrder == p).Stats.Find(s => s.GameId == Id).Stl += amt;
            }
            else if (s == "Blk")
            {
                teams[t].Players.Find(player => player.RosterOrder == p).Stats.Find(s => s.GameId == Id).Blk += amt;
            }
            else if (s == "Pf")
            {
                teams[t].Players.Find(player => player.RosterOrder == p).Stats.Find(s => s.GameId == Id).Pf += amt;
            }
            else if (s == "Pts")
            {
                teams[t].Players.Find(player => player.RosterOrder == p).Stats.Find(s => s.GameId == Id).Pts += amt;
            }
            else if (s == "Trb")
            {
                teams[t].Players.Find(player => player.RosterOrder == p).Stats.Find(s => s.GameId == Id).Trb += amt;
            }
            else if (s == "Gs")
            {
                teams[t].Players.Find(player => player.RosterOrder == p).Stats.Find(s => s.GameId == Id).Gs += amt;
            }
            else if (s == "CourtTime")
            {
                teams[t].Players.Find(player => player.RosterOrder == p).Stats.Find(s => s.GameId == Id).CourtTime += amntDouble;
            }
            else if (s == "BenchTime")
            {
                teams[t].Players.Find(player => player.RosterOrder == p).Stats.Find(s => s.GameId == Id).BenchTime += amntDouble;
            }
            else if (s == "Energy")
            {
                teams[t].Players.Find(player => player.RosterOrder == p).Stats.Find(s => s.GameId == Id).Energy += amntDouble;
            }
            else if (s == "Min")
            {
                teams[t].Players.Find(player => player.RosterOrder == p).Stats.Find(s => s.GameId == Id).Min += amntDouble;
            }

        }

        public static void RecordStatHelperPlayerMinutes(int t, int p, string s, int Id, Team[] teams, double amt = 1.0)
        {
            if (s == "CourtTime")
            {
                teams[t].Players.Find(player => player.RosterOrder == p).Stats.Find(s => s.GameId == Id).CourtTime += amt;
            }
            else if (s == "BenchTime")
            {
                teams[t].Players.Find(player => player.RosterOrder == p).Stats.Find(s => s.GameId == Id).BenchTime += amt;
            }
            else if (s == "Energy")
            {
                teams[t].Players.Find(player => player.RosterOrder == p).Stats.Find(s => s.GameId == Id).Energy += amt;
            }
            else if (s == "Min")
            {
                teams[t].Players.Find(player => player.RosterOrder == p).Stats.Find(s => s.GameId == Id).Min += amt;
            }
        }


    }
}
