using NBALigaSimulation.Shared.Engine;
using System.ComponentModel.DataAnnotations.Schema;

namespace NBALigaSimulation.Shared.Models
{
    public class Game
    {

        public int Id { get; set; }
        public int HomeTeamId { get; set; }
        [ForeignKey("HomeTeamId")]
        public Team HomeTeam { get; set; }
        public int AwayTeamId { get; set; }
        [ForeignKey("AwayTeamId")]
        public Team AwayTeam { get; set; }
        public int HomeQ1 { get; set; }
        public int HomeQ2 { get; set; }
        public int HomeQ3 { get; set; }
        public int HomeQ4 { get; set; }
        public int HomeOT { get; set; }
        public int AwayQ1 { get; set; }
        public int AwayQ2 { get; set; }
        public int AwayQ3 { get; set; }
        public int AwayQ4 { get; set; }
        public int AwayOT { get; set; }
        public ICollection<GamePlayByPlay> PlayByPlays { get; set; } = new HashSet<GamePlayByPlay>();

        //Sim
        [NotMapped]
        double synergyFactor = 0.1;
        [NotMapped]
        bool startersRecorded = false;
        [NotMapped]
        int subsEveryN = 6;
        [NotMapped]
        int overtimes = 0;
        [NotMapped]
        int t = 12;
        [NotMapped]
        int Dt = 12;
        [NotMapped]
        int o;
        [NotMapped]
        int d;
        [NotMapped]
        int NumPossessions;
        [NotMapped]
        int key;

        public void GameSim()
        {

            Team[] teams = { HomeTeam, AwayTeam };
            teams[0].CompositeRating = new TeamCompositeRating();
            teams[0].Stats.Add(new TeamGameStats());
            teams[0].Stats.LastOrDefault().Season = 2003;
            teams[0].Stats.LastOrDefault().GameId = Id;

            teams[1].CompositeRating = new TeamCompositeRating();
            teams[1].Stats.Add(new TeamGameStats());
            teams[1].Stats.LastOrDefault().Season = 2003;
            teams[1].Stats.LastOrDefault().GameId = Id;


            int[][] playersOnCourt = new int[2][] { HomeTeam.Players.Select(p => p.Id).ToArray(), AwayTeam.Players.Select(p => p.Id).ToArray() };
            this.NumPossessions = (int)Math.Round(((98 + 101) / 2) * new Random().NextDouble() * 0.2 + 0.9 * (98 + 101) / 2);
            this.Dt = 48 / (2 * this.NumPossessions);
            CompositeHelper.UpdateCompositeRating(teams, playersOnCourt);

            this.PlayByPlays.Add(new GamePlayByPlay
            {
                Play = "Start the Game!"
            });

            SimPossessions(teams, playersOnCourt);

        }

        public void SimPossessions(Team[] teams, int[][] playersOnCourt)
        {
            int i;
            string outcome;
            bool substitutions;

            this.o = 0;
            this.d = 1;

            i = 0;
            while (i < this.NumPossessions * 2)
            {

                // Clock
                this.t -= this.Dt;
                if (this.t < 0)
                {
                    this.t = 0;
                }

                // Possession change
                this.o = (this.o == 1) ? 0 : 1;
                this.d = (this.o == 1) ? 0 : 1;

                //UpdateTeamCompositeRatings();

                outcome = SimPossession(teams, playersOnCourt);

                // Swap o and d so that o will get another possession when they are swapped again at the beginning of the loop.
                if (outcome == "Orb")
                {
                    this.o = (this.o == 1) ? 0 : 1;
                    this.d = (this.o == 1) ? 0 : 1;
                }

                //this.updatePlayingTime();

                //this.injuries();

                /*
                if (i % this.subsEveryN == 0)
                {
                    substitutions = this.updatePlayersOnCourt();
                    if (substitutions)
                    {
                        this.updateSynergy();
                    }
                }
                */


                i += 1;
            }
        }

        public string SimPossession(Team[] teams, int[][] playersOnCourt)
        {

            Random random = new Random();

            // Turnover?
            if (DefenseHelper.ProbTov(teams) > random.NextDouble())
            {
                return DoTov(playersOnCourt, teams); // tov
            }

            // Shot if there is no turnover
            double[] ratios = ArrayHelper.RatingArray(playersOnCourt, teams, "GameUsage", this.o);
            int shooter = ArrayHelper.PickPlayer(ratios);

            return DoShot(shooter, playersOnCourt, teams); // fg, orb, or drb

        }

        private string DoTov(int[][] playersOnCourt, Team[] teams)
        {
            Random random = new Random();

            int o = this.o; // Índice da equipe ofensiva
            int p;
            double[] ratios;

            ratios = ArrayHelper.RatingArray(playersOnCourt, teams, "GameTurnovers", o, 0.5);
            p = playersOnCourt[o][ArrayHelper.PickPlayer(ratios)];
            RecordStat(o, p, "Tov", teams);

            if (DefenseHelper.ProbStl(teams) > random.NextDouble())
            {
                return DoStl(playersOnCourt, teams); // "stl"
            }

            return "Tov";
        }

        private string DoStl(int[][] playersOnCourt, Team[] teams)
        {
            int d = this.d;

            double[] ratios = ArrayHelper.RatingArray(playersOnCourt, teams, "GameStealing", d);
            int p = playersOnCourt[d][ArrayHelper.PickPlayer(ratios)];
            RecordStat(d, p, "Stl", teams);

            return "Stl";
        }

        public string DoShot(int shooter, int[][] playersOnCourt, Team[] teams)
        {
            Random random = new Random();

            int p = playersOnCourt[this.o][shooter];

            //double fatigue = Fatigue(team[o].player[p].stat.Energy);

            int passer = -1;
            if (ProbAst(teams) > random.NextDouble())
            {
                double[] ratios = ArrayHelper.RatingArray(playersOnCourt, teams, "GamePassing", o, 2);
                passer = ArrayHelper.PickPlayer(ratios, shooter);

                if (passer == shooter)
                {
                    passer = -1;
                }
            }

            double r1, r2, r3;
            string type;
            double probMissAndFoul, probMake, probAndOne;

            if (teams[this.o].Players.Find(play => play.Id == p).Ratings.LastOrDefault().GameShootingThreePointer > 0.4 && random.NextDouble()
                < (0.35 * teams[this.o].Players.Find(play => play.Id == p).Ratings.LastOrDefault().GameShootingThreePointer))
            {
                // Three pointer
                type = "ThreePointer";
                probMissAndFoul = 0.02;
                probMake = teams[this.o].Players.Find(play => play.Id == p).Ratings.LastOrDefault().GameShootingThreePointer * 0.68;
                probAndOne = 0.01;
            }
            else
            {
                r1 = random.NextDouble() * teams[this.o].Players.Find(play => play.Id == p).Ratings.LastOrDefault().GameShootingMidRange;
                //r2 = random.NextDouble() * (teams[this.o].Players[p].Ratings.LastOrDefault().GameShootingAtRim + this.synergyFactor * (this.team[this.o].synergy.off - this.team[this.d].synergy.def));
                r2 = random.NextDouble() * teams[this.o].Players.Find(play => play.Id == p).Ratings.LastOrDefault().GameShootingAtRim;
                r3 = random.NextDouble() * teams[this.o].Players.Find(play => play.Id == p).Ratings.LastOrDefault().GameShootingLowPost;

                if (r1 > r2 && r1 > r3)
                {
                    // Two point jumper
                    type = "MidRange";
                    probMissAndFoul = 0.07;
                    probMake = teams[this.o].Players.Find(play => play.Id == p).Ratings.LastOrDefault().GameShootingMidRange * 0.3 + 0.29;
                    probAndOne = 0.05;
                }
                else if (r2 > r3)
                {
                    // Dunk, fast break or half court
                    type = "AtRim";
                    probMissAndFoul = 0.37;
                    probMake = teams[this.o].Players.Find(play => play.Id == p).Ratings.LastOrDefault().GameShootingAtRim * 0.3 + 0.52;
                    probAndOne = 0.25;
                }
                else
                {
                    // Post up
                    type = "LowPost";
                    probMissAndFoul = 0.33;
                    probMake = teams[this.o].Players.Find(play => play.Id == p).Ratings.LastOrDefault().GameShootingLowPost * 0.3 + 0.37;
                    probAndOne = 0.15;
                }
            }

            //probMake = (probMake - 0.25 * this.team[this.d].compositeRating.defense + this.synergyFactor * (this.team[this.o].synergy.off - this.team[this.d].synergy.def)) * fatigue;
            probMake = (probMake - 0.25 * teams[this.d].CompositeRating.Ratings["GameDefense"] + this.synergyFactor);

            if (passer >= 0)
            {
                probMake += 0.025;
            }

            if (ProbBlk(teams) > random.NextDouble())
            {
                return DoBlk(shooter, type, playersOnCourt, teams);  // orb or drb
            }

            // Make
            if (probMake > random.NextDouble())
            {
                // And 1
                if (probAndOne > random.NextDouble())
                {
                    return DoFg(shooter, passer, type, true, playersOnCourt, teams);  // fg, orb, or drb
                }
                return DoFg(shooter, passer, type, false, playersOnCourt, teams);  // fg
            }

            // Miss, but fouled
            if (probMissAndFoul > random.NextDouble())
            {
                if (type == "ThreePointer")
                {
                    return DoFt(shooter, 3, playersOnCourt, teams);  // fg, orb, or drb
                }
                return DoFt(shooter, 2, playersOnCourt, teams);  // fg, orb, or drb
            }

            //Mis
            p = playersOnCourt[this.o][shooter];
            RecordStat(o, p, "Fga", teams);

            if (type == "AtRim")
            {
                RecordStat(o, p, "FgaAtRim", teams);
                RecordPlay("MissAtRim", this.o, new string[] { teams[this.o].Players.Find(play => play.Id == p).FullName });
            }
            else if (type == "LowPost")
            {
                RecordStat(o, p, "FgaLowPost", teams);
                RecordPlay("MissLowPost", this.o, new string[] { teams[this.o].Players.Find(play => play.Id == p).FullName });
            }
            else if (type == "MidRange")
            {
                RecordStat(o, p, "FgaMidRange", teams);
                RecordPlay("MissMidRange", this.o, new string[] { teams[this.o].Players.Find(play => play.Id == p).FullName });
            }
            else if (type == "ThreePointer")
            {
                RecordStat(o, p, "Tpa", teams);
                RecordPlay("MissTp", this.o, new string[] { teams[this.o].Players.Find(play => play.Id == p).FullName });
            }


            return DoReb(playersOnCourt, teams);

        }

        public string DoFg(int shooter, int passer, string type, bool andOne, int[][] playersOnCourt, Team[] teams)
        {
            int p;

            p = playersOnCourt[this.o][shooter];
            RecordStat(this.o, p, "Fga", teams);
            RecordStat(this.o, p, "Fg", teams);
            RecordStat(this.o, p, "Pts", teams, 2);  // 2 points for 2's

            if (type == "atRim")
            {
                RecordStat(this.o, p, "FgaAtRim", teams);
                RecordStat(this.o, p, "FgAtRim", teams);
                this.RecordPlay("FgAtRim" + (andOne ? "AndOne" : ""), this.o, new string[] { teams[this.o].Players.Find(play => play.Id == p).FullName });
            }
            else if (type == "LowPost")
            {
                RecordStat(this.o, p, "FgaLowPost", teams);
                RecordStat(this.o, p, "FgLowPost", teams);
                this.RecordPlay("FgLowPost" + (andOne ? "AndOne" : ""), this.o, new string[] { teams[this.o].Players.Find(play => play.Id == p).FullName });
            }
            else if (type == "MidRange")
            {
                RecordStat(this.o, p, "FgaMidRange", teams);
                RecordStat(this.o, p, "FgMidRange", teams);
                this.RecordPlay("FgMidRange" + (andOne ? "AndOne" : ""), this.o, new string[] { teams[this.o].Players.Find(play => play.Id == p).FullName });
            }
            else if (type == "ThreePointer")
            {
                RecordStat(this.o, p, "Pts", teams);  // Extra point for 3's
                RecordStat(this.o, p, "Tpa", teams);
                RecordStat(this.o, p, "Tp", teams);
                RecordPlay("Tp" + (andOne ? "AndOne" : ""), this.o, new string[] { teams[this.o].Players.Find(play => play.Id == p).FullName });
            }

            if (passer >= 0)
            {
                p = playersOnCourt[this.o][passer];
                RecordStat(this.o, p, "Ast", teams);
                RecordPlay("Ast", this.o, new string[] { teams[this.o].Players.Find(play => play.Id == p).FullName });
            }

            if (andOne)
            {
                return DoFt(shooter, 1, playersOnCourt, teams);  // fg, orb, or drb
            }

            return "Fg";
        }

        public string DoFt(int shooter, int amount, int[][] playersOnCourt, Team[] teams)
        {
            int i;
            string outcome = "";
            int p;

            DoPf(this.d, playersOnCourt, teams);
            p = playersOnCourt[this.o][shooter];

            for (i = 0; i < amount; i++)
            {
                RecordStat(this.o, p, "Fta", teams);

                if (new Random().NextDouble() < teams[this.o].Players.Find(play => play.Id == p).Ratings.LastOrDefault().GameShootingFT * 0.3 + 0.6)  // Between 60% and 90%
                {
                    RecordStat(this.o, p, "Ft", teams);
                    RecordStat(this.o, p, "Pts", teams);
                    RecordPlay("Ft", this.o, new string[] { teams[this.o].Players.Find(play => play.Id == p).FullName });
                    outcome = "Fg";
                }
                else
                {
                    this.RecordPlay("MissFt", this.o, new string[] { teams[this.o].Players.Find(play => play.Id == p).FullName });
                    outcome = null;
                }
            }

            if (outcome != "Fg")
            {
                outcome = this.DoReb(playersOnCourt, teams);  // orb or drb
            }

            return outcome;
        }

        public void DoPf(int t, int[][] playersOnCourt, Team[] teams)
        {
            int p;
            double[] ratios;

            ratios = ArrayHelper.RatingArray(playersOnCourt, teams, "Fouling", t);
            p = playersOnCourt[t][ArrayHelper.PickPlayer(ratios)];
            RecordStat(d, p, "Pf", teams);
            RecordPlay("Pf", this.d, new string[] { teams[this.d].Players.Find(play => play.Id == p).FullName });

            // Foul out
            /*
            if (teams[d].Player[p].Stat.Pf >= 6)
            {
                RecordPlay("foulOut", D, new string[] { teams[this.d].Players.Find(play => play.Id == p).FullName });

                // Force substitutions now
                UpdatePlayersOnCourt();
                UpdateSynergy();
            }
            */
        }


        public string DoBlk(int shooter, string type, int[][] playersOnCourt, Team[] teams)
        {
            var p = playersOnCourt[this.o][shooter];

            RecordStat(this.o, p, "Fga", teams);
            if (type == "AtRim")
            {
                RecordStat(this.o, p, "FgaAtRim", teams);
            }
            else if (type == "LowPost")
            {
                RecordStat(this.o, p, "FgaLowPost", teams);
            }
            else if (type == "MidRange")
            {
                RecordStat(this.o, p, "FgaMidRange", teams);
            }
            else if (type == "ThreePointer")
            {
                RecordStat(this.o, p, "Tpa", teams);
            }

            var ratios = ArrayHelper.RatingArray(playersOnCourt, teams, "GameBlocking", this.d, 4);
            var p2 = playersOnCourt[this.d][ArrayHelper.PickPlayer(ratios)];
            RecordStat(this.d, p2, "Blk", teams);

            if (type == "AtRim")
            {
                RecordPlay("BlkAtRim", this.d, new string[] { teams[this.d].Players.Find(pl => pl.Id == p2).FullName, teams[this.o].Players.Find(pl => pl.Id == p).FullName });
            }
            else if (type == "LowPost")
            {
                RecordPlay("BlkLowPost", this.d, new string[] { teams[this.d].Players.Find(pl => pl.Id == p2).FullName, teams[this.o].Players.Find(pl => pl.Id == p).FullName });
            }
            else if (type == "MidRange")
            {
                RecordPlay("BlkMidRange", this.d, new string[] { teams[this.d].Players.Find(pl => pl.Id == p2).FullName, teams[this.o].Players.Find(pl => pl.Id == p).FullName });
            }
            else if (type == "ThreePointer")
            {
                RecordPlay("BlkTp", this.d, new string[] { teams[this.d].Players.Find(pl => pl.Id == p2).FullName, teams[this.o].Players.Find(pl => pl.Id == p).FullName });
            }

            return DoReb(playersOnCourt, teams);  // orb or drb
        }

        public string DoReb(int[][] playersOnCourt, Team[] teams)
        {
            Random random = new Random();
            double randomValue = random.NextDouble();

            if (0.15 > randomValue)
            {
                return null;
            }

            double reboundingRatio = 0.75 * (2 + teams[this.d].CompositeRating.Ratings["GameRebounding"]) / (2 + teams[this.o].CompositeRating.Ratings["GameRebounding"]);
            if (reboundingRatio > random.NextDouble())
            {
                double[] ratios = ArrayHelper.RatingArray(playersOnCourt, teams, "GameRebounding", this.d);
                int PlayerIndex = playersOnCourt[this.d][ArrayHelper.PickPlayer(ratios)];
                RecordStat(this.d, PlayerIndex, "Drb", teams);
                RecordPlay("Drb", this.d, new string[] { teams[this.d].Players.Find(play => play.Id == PlayerIndex).FullName });

                return "Drb";
            }

            double[] opponentRatios = ArrayHelper.RatingArray(playersOnCourt, teams, "GameRebounding", this.o);
            int OPPlayerIndex = playersOnCourt[this.o][ArrayHelper.PickPlayer(opponentRatios)];
            RecordStat(this.o, OPPlayerIndex, "Orb", teams);
            this.RecordPlay("Orb", this.o, new string[] { teams[this.o].Players.Find(play => play.Id == OPPlayerIndex).FullName });

            return "Orb";
        }

        public double ProbAst(Team[] teams)
        {
            return 0.6 * (2 + teams[this.o].CompositeRating.Ratings["GamePassing"]) / (2 + teams[this.d].CompositeRating.Ratings["GameDefense"]);
        }

        public double ProbBlk(Team[] teams)
        {
            return 0.1 * teams[this.d].CompositeRating.Ratings["GameBlocking"];
        }

        public void RecordPlay(string type, int t, string[] names)
        {

            string[] texts = new string[0];

            if (type == "Injury")
            {
                texts = new string[] { "{0} was injured!" };
            }
            else if (type == "Tov")
            {
                texts = new string[] { "{0} turned the ball over" };
            }
            else if (type == "Stl")
            {
                texts = new string[] { "{0} stole the ball from {1}" };
            }
            else if (type == "FgAtRim")
            {
                texts = new string[] { "{0} made a dunk/layup" };
            }
            else if (type == "FgAtRimAndOne")
            {
                texts = new string[] { "{0} made a dunk/layup and got fouled!" };
            }
            else if (type == "FgLowPost")
            {
                texts = new string[] { "{0} made a low post shot" };
            }
            else if (type == "FgLowPostAndOne")
            {
                texts = new string[] { "{0} made a low post shot and got fouled!" };
            }
            else if (type == "FgMidRange")
            {
                texts = new string[] { "{0} made a mid-range shot" };
            }
            else if (type == "FgMidRangeAndOne")
            {
                texts = new string[] { "{0} made a mid-range shot and got fouled!" };
            }
            else if (type == "Tp")
            {
                texts = new string[] { "{0} made a three pointer shot" };
            }
            else if (type == "TpAndOne")
            {
                texts = new string[] { "{0} made a three pointer and got fouled!" };
            }
            else if (type == "BlkAtRim")
            {
                texts = new string[] { "{0} blocked {1}'s dunk/layup" };
            }
            else if (type == "BlkLowPost")
            {
                texts = new string[] { "{0} blocked {1}'s low post shot" };
            }
            else if (type == "BlkMidRange")
            {
                texts = new string[] { "{0} blocked {1}'s mid-range shot" };
            }
            else if (type == "BlkTp")
            {
                texts = new string[] { "{0} blocked {1}'s three pointer" };
            }
            else if (type == "MissAtRim")
            {
                texts = new string[] { "{0} missed a dunk/layup" };
            }
            else if (type == "MissLowPost")
            {
                texts = new string[] { "{0} missed a low post shot" };
            }
            else if (type == "MissMidRange")
            {
                texts = new string[] { "{0} missed a mid-range shot" };
            }
            else if (type == "MissTp")
            {
                texts = new string[] { "{0} missed a three pointer" };
            }
            else if (type == "Orb")
            {
                texts = new string[] { "{0} grabbed the offensive rebound" };
            }
            else if (type == "Drb")
            {
                texts = new string[] { "{0} grabbed the defensive rebound" };
            }
            else if (type == "Ast")
            {
                texts = new string[] { "(assist: {0})" };
            }
            else if (type == "Quarter")
            {
                //texts = new string[] { "<b>Start of " + helpers.ordinal(this.team[0].stat.ptsQtrs.length) + " quarter</b>" };
                texts = new string[] { "<b>Start of quarter</b>" };
            }
            else if (type == "Overtime")
            {
                //texts = new string[] { "<b>Start of " + helpers.ordinal(this.team[0].stat.ptsQtrs.length - 4) + " overtime period</b>" };
                texts = new string[] { "<b>Start of overtime period</b>" };
            }
            else if (type == "Ft")
            {
                texts = new string[] { "{0} made a free throw" };
            }
            else if (type == "MissFt")
            {
                texts = new string[] { "{0} missed a free throw" };
            }
            else if (type == "Pf")
            {
                texts = new string[] { "Foul on {0}" };
            }
            else if (type == "FoulOut")
            {
                texts = new string[] { "{0} fouled out" };
            }
            else if (type == "Sub")
            {
                texts = new string[] { "Substitution: {0} for {1}" };
            }


            string text = "";
            if (texts != null && texts.Length > 0)
            {
                text = texts[0];
                if (names != null)
                {
                    for (int i = 0; i < names.Length; i++)
                    {
                        text = text.Replace("{" + i + "}", names[i]);
                    }
                }
            }

            this.key += 1;

            double sec = Math.Floor((double)this.t % 1 * 60);
            this.PlayByPlays.Add(new GamePlayByPlay
            {
                Play = text + " " + key
            });



        }


        public void RecordStat(int t, int p, string s, Team[] teams, int amt = 1)
        {
            amt = amt != default ? amt : 1;

            RecordStatHelperPlayer(t, p, s, teams, amt);
            if (s != "Gs" && s != "CourtTime" && s != "BenchTime" && s != "Energy")
            {
                RecordStatHelperTeam(t, p, s, teams, amt);
                // Record quarter-by-quarter scoring too
                /*
                if (s == "Pts")
                {
                    this.team[t].stat.ptsQtrs[this.team[t].stat.ptsQtrs.Count - 1] += amt;
                }
                if (this.playByPlay != default)
                {
                    this.playByPlay.Add(new PlayByPlayEntry
                    {
                        Type = "stat",
                        Qtr = this.team[t].stat.ptsQtrs.Count - 1,
                        T = t,
                        P = p,
                        S = s,
                        Amt = amt
                    });
                }
                */
            }
        }

        public void RecordStatHelperTeam(int t, int p, string s, Team[] teams, int amt = 1)
        {

            if (s == "Fg")
            {
                teams[t].Stats.LastOrDefault().Fg += amt;
            }
            else if (s == "Fga")
            {
                teams[t].Stats.LastOrDefault().Fga += amt;
            }
            else if (s == "FgAtRim")
            {
                teams[t].Stats.LastOrDefault().FgAtRim += amt;
            }
            else if (s == "FgaAtRim")
            {
                teams[t].Stats.LastOrDefault().FgaAtRim += amt;
            }
            else if (s == "FgLowPost")
            {
                teams[t].Stats.LastOrDefault().FgLowPost += amt;
            }
            else if (s == "FgaLowPost")
            {
                teams[t].Stats.LastOrDefault().FgaLowPost += amt;
            }
            else if (s == "FgMidRange")
            {
                teams[t].Stats.LastOrDefault().FgMidRange += amt;
            }
            else if (s == "FgaMidRange")
            {
                teams[t].Stats.LastOrDefault().FgaMidRange += amt;
            }
            else if (s == "Tp")
            {
                teams[t].Stats.LastOrDefault().Tp += amt;
            }
            else if (s == "Tpa")
            {
                teams[t].Stats.LastOrDefault().Tpa += amt;
            }
            else if (s == "Ft")
            {
                teams[t].Stats.LastOrDefault().Ft += amt;
            }
            else if (s == "Fta")
            {
                teams[t].Stats.LastOrDefault().Fta += amt;
            }
            else if (s == "Orb")
            {
                teams[t].Stats.LastOrDefault().Orb += amt;
            }
            else if (s == "Drb")
            {
                teams[t].Stats.LastOrDefault().Drb += amt;
            }
            else if (s == "Ast")
            {
                teams[t].Stats.LastOrDefault().Ast += amt;
            }
            else if (s == "Tov")
            {
                teams[t].Stats.LastOrDefault().Tov += amt;
            }
            else if (s == "Stl")
            {
                teams[t].Stats.LastOrDefault().Stl += amt;
            }
            else if (s == "Blk")
            {
                teams[t].Stats.LastOrDefault().Blk += amt;
            }
            else if (s == "Pf")
            {
                teams[t].Stats.LastOrDefault().Pf += amt;
            }
            else if (s == "Pts")
            {
                teams[t].Stats.LastOrDefault().Pts += amt;
            }
            else if (s == "Trb")
            {
                teams[t].Stats.LastOrDefault().Trb += amt;
            }

        }


        public void RecordStatHelperPlayer(int t, int p, string s, Team[] teams, int GameId, int amt = 1)
        {

            if (teams[t].Players.Find(player => player.Id == p).Stats.LastOrDefault() == null)
            {
                teams[t].Players.Find(player => player.Id == p).Stats.Add(new PlayerGameStats());
                teams[t].Players.Find(player => player.Id == p).Stats.LastOrDefault().GameId = GameId;

            }


            if (s == "Fg")
            {
                teams[t].Players.Find(player => player.Id == p).Stats.LastOrDefault().Fg += amt;
            }
            else if (s == "Fga")
            {
                teams[t].Players.Find(player => player.Id == p).Stats.LastOrDefault().Fga += amt;
            }
            else if (s == "FgAtRim")
            {
                teams[t].Players.Find(player => player.Id == p).Stats.LastOrDefault().FgAtRim += amt;
            }
            else if (s == "FgaAtRim")
            {
                teams[t].Players.Find(player => player.Id == p).Stats.LastOrDefault().FgaAtRim += amt;
            }
            else if (s == "FgLowPost")
            {
                teams[t].Players.Find(player => player.Id == p).Stats.LastOrDefault().FgLowPost += amt;
            }
            else if (s == "FgaLowPost")
            {
                teams[t].Players.Find(player => player.Id == p).Stats.LastOrDefault().FgaLowPost += amt;
            }
            else if (s == "FgMidRange")
            {
                teams[t].Players.Find(player => player.Id == p).Stats.LastOrDefault().FgMidRange += amt;
            }
            else if (s == "FgaMidRange")
            {
                teams[t].Players.Find(player => player.Id == p).Stats.LastOrDefault().FgaMidRange += amt;
            }
            else if (s == "Tp")
            {
                teams[t].Players.Find(player => player.Id == p).Stats.LastOrDefault().Tp += amt;
            }
            else if (s == "Tpa")
            {
                teams[t].Players.Find(player => player.Id == p).Stats.LastOrDefault().Tpa += amt;
            }
            else if (s == "Ft")
            {
                teams[t].Players.Find(player => player.Id == p).Stats.LastOrDefault().Ft += amt;
            }
            else if (s == "Fta")
            {
                teams[t].Players.Find(player => player.Id == p).Stats.LastOrDefault().Fta += amt;
            }
            else if (s == "Orb")
            {
                teams[t].Players.Find(player => player.Id == p).Stats.LastOrDefault().Orb += amt;
            }
            else if (s == "Drb")
            {
                teams[t].Players.Find(player => player.Id == p).Stats.LastOrDefault().Drb += amt;
            }
            else if (s == "Ast")
            {
                teams[t].Players.Find(player => player.Id == p).Stats.LastOrDefault().Ast += amt;
            }
            else if (s == "Tov")
            {
                teams[t].Players.Find(player => player.Id == p).Stats.LastOrDefault().Tov += amt;
            }
            else if (s == "Stl")
            {
                teams[t].Players.Find(player => player.Id == p).Stats.LastOrDefault().Stl += amt;
            }
            else if (s == "Blk")
            {
                teams[t].Players.Find(player => player.Id == p).Stats.LastOrDefault().Blk += amt;
            }
            else if (s == "Pf")
            {
                teams[t].Players.Find(player => player.Id == p).Stats.LastOrDefault().Pf += amt;
            }
            else if (s == "Pts")
            {
                teams[t].Players.Find(player => player.Id == p).Stats.LastOrDefault().Pts += amt;
            }
            else if (s == "Trb")
            {
                teams[t].Players.Find(player => player.Id == p).Stats.LastOrDefault().Trb += amt;
            }

        }

    }
}