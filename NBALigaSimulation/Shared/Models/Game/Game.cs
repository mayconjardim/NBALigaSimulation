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
        int o;
        [NotMapped]
        int d;


        public void GameSim()
        {

            Team[] teams = { HomeTeam, AwayTeam };
            teams[0].CompositeRating = new TeamCompositeRating();
            teams[1].CompositeRating = new TeamCompositeRating();
            int numPossessions = (int)Math.Round(((98 + 101) / 2) * new Random().NextDouble() * 0.2 + 0.9 * (98 + 101) / 2);
            int[][] playersOnCourt = new int[2][] { HomeTeam.Players.Select(p => p.Id).ToArray(), AwayTeam.Players.Select(p => p.Id).ToArray() };

            CompositeHelper.UpdateCompositeRating(teams, playersOnCourt);

            this.PlayByPlays.Add(new GamePlayByPlay
            {
                Play = "Start the Game!"
            });

            SimPossessions(numPossessions, teams, playersOnCourt);

        }

        public void SimPossessions(int numPossessions, Team[] teams, int[][] playersOnCourt)
        {
            int i = 0;
            while (i < numPossessions * 2)
            {
                this.o = (this.o == 1) ? 0 : 1;
                this.d = (this.o == 1) ? 0 : 1;

                //if (i % subsEveryN == 0)
                //{
                //    bool substitutions = UpdatePlayersOnCourt();
                //    if (substitutions)
                //    {
                //        UpdateSynergy();
                //    }
                //}

                //UpdateTeamCompositeRatings(teams, playersOnCourt);

                string outcome = SimPossession(teams, playersOnCourt);

                // Swap o and d so that o will get another possession when they are swapped again at the beginning of the loop.
                if (outcome == "orb")
                {
                    this.o = (this.o == 1) ? 0 : 1;
                    this.d = (this.o == 1) ? 0 : 1;
                }

                //UpdatePlayingTime();

                //Injuries();

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
            // RecordStat(o, p, "tov");

            if (DefenseHelper.ProbStl(teams) > random.NextDouble())
            {
                return DoStl(playersOnCourt, teams); // "stl"
            }

            return "tov";
        }

        private string DoStl(int[][] playersOnCourt, Team[] teams)
        {
            int d = this.d;

            double[] ratios = ArrayHelper.RatingArray(playersOnCourt, teams, "GameStealing", d);
            int p = playersOnCourt[d][ArrayHelper.PickPlayer(ratios)];
            //RecordStat(d, p, "stl");

            return "stl";
        }

        public string DoShot(int shooter, int[][] playersOnCourt, Team[] teams)
        {
            Random random = new Random();

            int p = playersOnCourt[this.o][shooter];

            //double fatigue = Fatigue(team[o].player[p].stat.energy);

            int passer = -1;
            if (ProbAst(teams) > random.NextDouble())
            {
                double[] ratios = ArrayHelper.RatingArray(playersOnCourt, teams, "passing", o, 2);
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
                type = "threePointer";
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
                    type = "midRange";
                    probMissAndFoul = 0.07;
                    probMake = teams[this.o].Players.Find(play => play.Id == p).Ratings.LastOrDefault().GameShootingMidRange * 0.3 + 0.29;
                    probAndOne = 0.05;
                }
                else if (r2 > r3)
                {
                    // Dunk, fast break or half court
                    type = "atRim";
                    probMissAndFoul = 0.37;
                    probMake = teams[this.o].Players.Find(play => play.Id == p).Ratings.LastOrDefault().GameShootingAtRim * 0.3 + 0.52;
                    probAndOne = 0.25;
                }
                else
                {
                    // Post up
                    type = "lowPost";
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
                if (type == "threePointer")
                {
                    return DoFt(shooter, 3, playersOnCourt, teams);  // fg, orb, or drb
                }
                return DoFt(shooter, 2, playersOnCourt, teams);  // fg, orb, or drb
            }

            //Mis
            p = playersOnCourt[this.o][shooter];
            //RecordStat(O, p, "fga");

            if (type == "atRim")
            {
                //RecordStat(O, p, "fgaAtRim");
                RecordPlay("missAtRim", this.o, new string[] { teams[this.o].Players.Find(play => play.Id == p).FullName });
            }
            else if (type == "lowPost")
            {
                //RecordStat(O, p, "fgaLowPost");
                RecordPlay("missLowPost", this.o, new string[] { teams[this.o].Players.Find(play => play.Id == p).FullName });
            }
            else if (type == "midRange")
            {
                //RecordStat(O, p, "fgaMidRange");
                RecordPlay("missMidRange", this.o, new string[] { teams[this.o].Players.Find(play => play.Id == p).FullName });
            }
            else if (type == "threePointer")
            {
                //RecordStat(O, p, "tpa");
                RecordPlay("missTp", this.o, new string[] { teams[this.o].Players.Find(play => play.Id == p).FullName });
            }


            return DoReb(playersOnCourt, teams);

        }

        public string DoFg(int shooter, int passer, string type, bool andOne, int[][] playersOnCourt, Team[] teams)
        {
            int p;

            p = playersOnCourt[this.o][shooter];
            // this.RecordStat(this.O, p, "fga");
            //this.RecordStat(this.O, p, "fg");
            //this.RecordStat(this.O, p, "pts", 2);  // 2 points for 2's

            if (type == "atRim")
            {
                // this.RecordStat(this.O, p, "fgaAtRim");
                //this.RecordStat(this.O, p, "fgAtRim");
                this.RecordPlay("fgAtRim" + (andOne ? "AndOne" : ""), this.o, new string[] { teams[this.o].Players.Find(play => play.Id == p).FullName });
            }
            else if (type == "lowPost")
            {
                //this.RecordStat(this.O, p, "fgaLowPost");
                //this.RecordStat(this.O, p, "fgLowPost");
                this.RecordPlay("fgLowPost" + (andOne ? "AndOne" : ""), this.o, new string[] { teams[this.o].Players.Find(play => play.Id == p).FullName });
            }
            else if (type == "midRange")
            {
                // this.RecordStat(this.O, p, "fgaMidRange");
                //this.RecordStat(this.O, p, "fgMidRange");
                this.RecordPlay("fgMidRange" + (andOne ? "AndOne" : ""), this.o, new string[] { teams[this.o].Players.Find(play => play.Id == p).FullName });
            }
            else if (type == "threePointer")
            {
                // this.RecordStat(this.O, p, "pts");  // Extra point for 3's
                //this.RecordStat(this.O, p, "tpa");
                // this.RecordStat(this.O, p, "tp");
                this.RecordPlay("tp" + (andOne ? "AndOne" : ""), this.o, new string[] { teams[this.o].Players.Find(play => play.Id == p).FullName });
            }

            if (passer >= 0)
            {
                p = playersOnCourt[this.o][passer];
                //  this.RecordStat(this.O, p, "ast");
                this.RecordPlay("ast", this.o, new string[] { teams[this.o].Players.Find(play => play.Id == p).FullName });
            }

            if (andOne)
            {
                return DoFt(shooter, 1, playersOnCourt, teams);  // fg, orb, or drb
            }

            return "fg";
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
                //  this.RecordStat(this.O, p, "fta");

                if (new Random().NextDouble() < teams[this.o].Players.Find(play => play.Id == p).Ratings.LastOrDefault().GameShootingFT * 0.3 + 0.6)  // Between 60% and 90%
                {
                    //this.RecordStat(this.O, p, "ft");
                    //this.RecordStat(this.O, p, "pts");
                    //this.RecordPlay("ft", this.O, new string[] { this.Team[this.O].Player[p].Name });
                    outcome = "fg";
                }
                else
                {
                    this.RecordPlay("missFt", this.o, new string[] { teams[this.o].Players.Find(play => play.Id == p).FullName });
                    outcome = null;
                }
            }

            if (outcome != "fg")
            {
                outcome = this.DoReb(playersOnCourt, teams);  // orb or drb
            }

            return outcome;
        }

        public void DoPf(int t, int[][] playersOnCourt, Team[] teams)
        {
            int p;
            double[] ratios;

            ratios = ArrayHelper.RatingArray(playersOnCourt, teams, "fouling", t);
            p = playersOnCourt[t][ArrayHelper.PickPlayer(ratios)];
            // RecordStat(D, p, "pf");
            RecordPlay("pf", this.d, new string[] { teams[this.d].Players.Find(play => play.Id == p).FullName });

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

            /*
             this.recordStat(this.o, p, "fga");
             if (type == "atRim")
             {
                 this.recordStat(this.o, p, "fgaAtRim");
             }
             else if (type == "lowPost")
             {
                 this.recordStat(this.o, p, "fgaLowPost");
             }
             else if (type == "midRange")
             {
                 this.recordStat(this.o, p, "fgaMidRange");
             }
             else if (type == "threePointer")
             {
                 this.recordStat(this.o, p, "tpa");
             }


             this.recordStat(this.d, p2, "blk");
             */

            var ratios = ArrayHelper.RatingArray(playersOnCourt, teams, "blocking", this.d, 4);
            var p2 = playersOnCourt[this.d][ArrayHelper.PickPlayer(ratios)];

            if (type == "atRim")
            {
                RecordPlay("blkAtRim", this.d, new string[] { teams[this.d].Players.Find(pl => pl.Id == p2).FullName, teams[this.o].Players.Find(pl => pl.Id == p).FullName });
            }
            else if (type == "lowPost")
            {
                RecordPlay("blkLowPost", this.d, new string[] { teams[this.d].Players.Find(pl => pl.Id == p2).FullName, teams[this.o].Players.Find(pl => pl.Id == p).FullName });
            }
            else if (type == "midRange")
            {
                RecordPlay("blkMidRange", this.d, new string[] { teams[this.d].Players.Find(pl => pl.Id == p2).FullName, teams[this.o].Players.Find(pl => pl.Id == p).FullName });
            }
            else if (type == "threePointer")
            {
                RecordPlay("blkTp", this.d, new string[] { teams[this.d].Players.Find(pl => pl.Id == p2).FullName, teams[this.o].Players.Find(pl => pl.Id == p).FullName });
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
                //this.RecordStat(this.D, playerIndex, "drb");
                this.RecordPlay("drb", this.d, new string[] { teams[this.d].Players.Find(play => play.Id == PlayerIndex).FullName });

                return "drb";
            }

            double[] opponentRatios = ArrayHelper.RatingArray(playersOnCourt, teams, "GameRebounding", this.o);
            int OPPlayerIndex = playersOnCourt[this.o][ArrayHelper.PickPlayer(opponentRatios)];
            //this.RecordStat(this.O, opponentPlayerIndex, "orb");
            this.RecordPlay("orb", this.o, new string[] { teams[this.o].Players.Find(play => play.Id == OPPlayerIndex).FullName });

            return "orb";
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

            if (type == "injury")
            {
                texts = new string[] { "{0} was injured!" };
            }
            else if (type == "tov")
            {
                texts = new string[] { "{0} turned the ball over" };
            }
            else if (type == "stl")
            {
                texts = new string[] { "{0} stole the ball from {1}" };
            }
            else if (type == "fgAtRim")
            {
                texts = new string[] { "{0} made a dunk/layup" };
                if (t == 0)
                {
                    AwayQ1 += 2;
                }
                else
                {
                    HomeQ1 += 2;
                }
            }
            else if (type == "fgAtRimAndOne")
            {
                texts = new string[] { "{0} made a dunk/layup and got fouled!" };
                if (t == 0)
                {
                    AwayQ1 += 2;
                }
                else
                {
                    HomeQ1 += 2;
                }
            }
            else if (type == "fgLowPost")
            {
                texts = new string[] { "{0} made a low post shot" };
                if (t == 0)
                {
                    AwayQ1 += 2;
                }
                else
                {
                    HomeQ1 += 2;
                }
            }
            else if (type == "fgLowPostAndOne")
            {
                texts = new string[] { "{0} made a low post shot and got fouled!" };
                if (t == 0)
                {
                    AwayQ1 += 2;
                }
                else
                {
                    HomeQ1 += 2;
                }
            }
            else if (type == "fgMidRange")
            {
                texts = new string[] { "{0} made a mid-range shot" };
                if (t == 0)
                {
                    AwayQ1 += 2;
                }
                else
                {
                    HomeQ1 += 2;
                }
            }
            else if (type == "fgMidRangeAndOne")
            {
                texts = new string[] { "{0} made a mid-range shot and got fouled!" };
                if (t == 0)
                {
                    AwayQ1 += 2;
                }
                else
                {
                    HomeQ1 += 2;
                }
            }
            else if (type == "tp")
            {
                texts = new string[] { "{0} made a three pointer shot" };
                if (t == 0)
                {
                    AwayQ1 += 3;
                }
                else
                {
                    HomeQ1 += 3;
                }
            }
            else if (type == "tpAndOne")
            {
                texts = new string[] { "{0} made a three pointer and got fouled!" };
                if (t == 0)
                {
                    AwayQ1 += 3;
                }
                else
                {
                    HomeQ1 += 3;
                }
            }
            else if (type == "blkAtRim")
            {
                texts = new string[] { "{0} blocked {1}'s dunk/layup" };
            }
            else if (type == "blkLowPost")
            {
                texts = new string[] { "{0} blocked {1}'s low post shot" };
            }
            else if (type == "blkMidRange")
            {
                texts = new string[] { "{0} blocked {1}'s mid-range shot" };
            }
            else if (type == "blkTp")
            {
                texts = new string[] { "{0} blocked {1}'s three pointer" };
            }
            else if (type == "missAtRim")
            {
                texts = new string[] { "{0} missed a dunk/layup" };
            }
            else if (type == "missLowPost")
            {
                texts = new string[] { "{0} missed a low post shot" };
            }
            else if (type == "missMidRange")
            {
                texts = new string[] { "{0} missed a mid-range shot" };
            }
            else if (type == "missTp")
            {
                texts = new string[] { "{0} missed a three pointer" };
            }
            else if (type == "orb")
            {
                texts = new string[] { "{0} grabbed the offensive rebound" };
            }
            else if (type == "drb")
            {
                texts = new string[] { "{0} grabbed the defensive rebound" };
            }
            else if (type == "ast")
            {
                texts = new string[] { "(assist: {0})" };
            }
            else if (type == "quarter")
            {
                //texts = new string[] { "<b>Start of " + helpers.ordinal(this.team[0].stat.ptsQtrs.length) + " quarter</b>" };
                texts = new string[] { "<b>Start of quarter</b>" };
            }
            else if (type == "overtime")
            {
                //texts = new string[] { "<b>Start of " + helpers.ordinal(this.team[0].stat.ptsQtrs.length - 4) + " overtime period</b>" };
                texts = new string[] { "<b>Start of overtime period</b>" };
            }
            else if (type == "ft")
            {
                texts = new string[] { "{0} made a free throw" };

                if (t == 0)
                {
                    AwayQ1 += 1;
                }
                else
                {
                    HomeQ1 += 1;
                }

            }
            else if (type == "missFt")
            {
                texts = new string[] { "{0} missed a free throw" };
            }
            else if (type == "pf")
            {
                texts = new string[] { "Foul on {0}" };
            }
            else if (type == "foulOut")
            {
                texts = new string[] { "{0} fouled out" };
            }
            else if (type == "sub")
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


            this.PlayByPlays.Add(new GamePlayByPlay
            {
                Play = text
            });



        }


    }
}