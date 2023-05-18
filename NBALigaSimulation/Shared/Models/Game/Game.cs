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
        public List<TeamGameStats> TeamGameStats { get; set; }
        public List<PlayerGameStats> PlayerGameStats { get; set; }
        public List<GamePlayByPlay> PlayByPlay { get; set; }
        //Atributos Globais da Simulação
        [NotMapped]
        int NumPossessions; // Quantidade posses de uma partida
        [NotMapped]
        bool StartersRecorded = false;
        [NotMapped]
        int SubsEveryN = 6; // Quantas posses esperar antes de fazer substituições
        [NotMapped]
        int Overtimes = 0; // Números de overtimes 
        [NotMapped]
        double SynergyFactor = 0.1; //Qual a importância da sinergia?
        [NotMapped]
        int Offense; //Time que está atacando
        [NotMapped]
        int Defense; //Time que está defendendo
        [NotMapped]
        int T = 12;
        [NotMapped]
        int Dt;

        //Simulação
        public void GameSim()
        {
            //Atribui os times para simulação
            Team[] teams = { HomeTeam, AwayTeam };

            //Gerando a quantidade posses de bola tem uma partida *(98 + 101 substituir por pace dos times) 
            Random random = new Random();
            double randomFactor = random.NextDouble() * (1.1 - 0.9) + 0.9;
            NumPossessions = (int)Math.Round(92.7 + 98.7 / 2 * randomFactor);
            Console.WriteLine(NumPossessions);
            // Escalações iniciais, que serão redefinidas por updatePlayersOnCourt. Isso deve ser feito por causa dos jogadores lesionados no top 5.
            int[][] playersOnCourt = new int[][] { new int[] { 0, 1, 2, 3, 4 }, new int[] { 0, 1, 2, 3, 4 } };

            // Simula o jogo até o fim (sem OT)
            SimPossessions(teams, playersOnCourt);

        }

        public void SimPossessions(Team[] teams, int[][] playersOnCourt)
        {
            int i = 0;
            string outcome;
            bool substitutions;

            Offense = 0;
            Defense = 1;

            while (i < NumPossessions * 2)
            {
                // Troca de posse
                Offense = (Offense == 1) ? 0 : 1;
                Defense = (Defense == 1) ? 0 : 1;


                if (i % SubsEveryN == 0)
                {
                    substitutions = UpdatePlayersOnCourt(teams, playersOnCourt);
                    if (substitutions != null)
                    {
                        //UpdateSynergy();
                    }
                }

                CompositeHelper.UpdateCompositeRating(teams, playersOnCourt);

                outcome = SimPossession(teams, playersOnCourt);

                // Troca Offense e Defense para que o receba outra posse quando eles forem trocados novamente no início do loop.
                if (outcome == "Orb")
                {
                    Offense = (Offense == 1) ? 0 : 1;
                    Defense = (Defense == 1) ? 0 : 1;
                }

                UpdatePlayingTime(playersOnCourt, teams);

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
                return DoTov(teams, playersOnCourt); // tov
            }

            // Shot if there is no turnover
            double[] ratios = RatingArray(playersOnCourt, teams, "GameUsage", Offense);
            int shooter = ArrayHelper.PickPlayer(ratios);

            return DoShot(shooter, teams, playersOnCourt); // fg, orb, or drb
        }

        public string DoTov(Team[] teams, int[][] playersOnCourt)
        {

            Random random = new Random();

            double[] ratios = RatingArray(playersOnCourt, teams, "GameTurnovers", Offense, 0.5);
            int p = playersOnCourt[Offense][ArrayHelper.PickPlayer(ratios)];
            RecordStat(Offense, p, "Tov", playersOnCourt, teams);

            if (DefenseHelper.ProbStl(teams) > random.NextDouble())
            {
                return DoStl(teams, playersOnCourt);  // "stl"
            }

            return "Tov";
        }

        public string DoStl(Team[] teams, int[][] playersOnCourt)
        {
            int p;
            double[] ratios;

            ratios = RatingArray(playersOnCourt, teams, "GameStealing", Defense);
            p = playersOnCourt[Defense][ArrayHelper.PickPlayer(ratios)];
            RecordStat(Defense, p, "Stl", playersOnCourt, teams);

            return "Stl";
        }


        public string DoShot(int shooter, Team[] teams, int[][] playersOnCourt)
        {

            Random random = new Random();

            double fatigue;
            int p, passer;
            double probMake, probAndOne, probMissAndFoul, r1, r2, r3;
            double[] ratios;
            string type;

            p = playersOnCourt[Offense][shooter];

            fatigue = Fatigue(teams[Offense].Players.Find(player => player.RosterOrder == p).Stats.Find(s => s.GameId == Id).Energy);

            // Esta é uma tentativa "assistencia" (ou seja, uma assistência será registrada se for feita)
            passer = -1;
            if (OffenseHelper.ProbAst(teams, Offense, Defense) > random.NextDouble())
            {
                ratios = RatingArray(playersOnCourt, teams, "GamePassing", Offense, 2);
                passer = ArrayHelper.PickPlayer(ratios, shooter);
            }

            if (teams[Offense].Players.Find(player => player.RosterOrder == p).Ratings.LastOrDefault().GameShootingThreePointer > 0.4 && random.NextDouble() < (0.35 * teams[Offense].Players.Find(player => player.RosterOrder == p).Ratings.LastOrDefault().GameShootingThreePointer))
            {
                // Three pointer
                type = "ThreePointer";
                probMissAndFoul = 0.02;
                probMake = teams[Offense].Players.Find(player => player.RosterOrder == p).Ratings.LastOrDefault().GameShootingThreePointer * 0.68;
                probAndOne = 0.01;
            }
            else
            {
                r1 = random.NextDouble() * teams[Offense].Players.Find(player => player.RosterOrder == p).Ratings.LastOrDefault().GameShootingMidRange;
                //double r2 = random.NextDouble() * (team[o].player[p].compositeRating.shootingAtRim + SynergyFactor * (team[o].synergy.off - team[d].synergy.def));
                r2 = random.NextDouble() * (teams[Offense].Players.Find(player => player.RosterOrder == p).Ratings.LastOrDefault().GameShootingAtRim + SynergyFactor);
                r3 = random.NextDouble() * teams[Offense].Players.Find(player => player.RosterOrder == p).Ratings.LastOrDefault().GameShootingLowPost;
                if (r1 > r2 && r1 > r3)
                {
                    // Two point jumper
                    type = "MidRange";
                    probMissAndFoul = 0.07;
                    probMake = teams[Offense].Players.Find(player => player.RosterOrder == p).Ratings.LastOrDefault().GameShootingMidRange * 0.3 + 0.29;
                    probAndOne = 0.05;
                }
                else if (r2 > r3)
                {
                    // Dunk, fast break or half court
                    type = "AtRim";
                    probMissAndFoul = 0.37;
                    probMake = teams[Offense].Players.Find(player => player.RosterOrder == p).Ratings.LastOrDefault().GameShootingAtRim * 0.3 + 0.52;
                    probAndOne = 0.25;
                }
                else
                {
                    // Post up
                    type = "LowPost";
                    probMissAndFoul = 0.33;
                    probMake = teams[Offense].Players.Find(player => player.RosterOrder == p).Ratings.LastOrDefault().GameShootingLowPost * 0.3 + 0.37;
                    probAndOne = 0.15;
                }
            }

            //probMake = (probMake - 0.25 * this.team[this.d].compositeRating.defense + this.synergyFactor * (this.team[this.o].synergy.off - this.team[this.d].synergy.def)) * fatigue;
            probMake = (probMake - 0.25 * teams[Defense].CompositeRating.Ratings["GameDefense"] + SynergyFactor * fatigue);

            if (passer >= 0)
            {
                probMake += 0.025;
            }

            if (DefenseHelper.ProbBlk(teams, Defense) > random.NextDouble())
            {
                return DoBlk(shooter, teams, playersOnCourt);  // orb or drb
            }

            // Make
            if (probMake > random.NextDouble())
            {
                // And 1
                if (probAndOne > random.NextDouble())
                {
                    DoFg(shooter, passer, type, teams, playersOnCourt);
                    return DoFt(shooter, 1, teams, playersOnCourt);  // fg, orb, or drb
                }
                return DoFg(shooter, passer, type, teams, playersOnCourt);  // fg
            }

            // Miss, but fouled
            if (probMissAndFoul > random.NextDouble())
            {
                if (type == "ThreePointer")
                {
                    return DoFt(shooter, 3, teams, playersOnCourt);  // fg, orb, or drb
                }
                return DoFt(shooter, 2, teams, playersOnCourt);  // fg, orb, or drb
            }

            //Miss
            p = playersOnCourt[Offense][shooter];
            RecordStat(Offense, p, "Fga", playersOnCourt, teams);
            if (type == "AtRim")
            {
                RecordStat(Offense, p, "FgaAtRim", playersOnCourt, teams);
            }
            else if (type == "LowPost")
            {
                RecordStat(Offense, p, "FgaLowPost", playersOnCourt, teams);
            }
            else if (type == "MidRange")
            {
                RecordStat(Offense, p, "FgaMidRange", playersOnCourt, teams);
            }
            else if (type == "ThreePointer")
            {
                RecordStat(Offense, p, "Tpa", playersOnCourt, teams);
            }

            return DoReb(teams, playersOnCourt); // orb or drb
        }

        private string DoBlk(int shooter, Team[] teams, int[][] playersOnCourt)
        {
            int p;
            double[] ratios;

            ratios = RatingArray(playersOnCourt, teams, "GameBlocking", Defense, 4);
            p = playersOnCourt[Defense][ArrayHelper.PickPlayer(ratios)];
            RecordStat(Defense, p, "Blk", playersOnCourt, teams);

            p = playersOnCourt[Offense][shooter];
            RecordStat(Offense, p, "Fga", playersOnCourt, teams);

            return DoReb(teams, playersOnCourt);  // orb or drb
        }

        private string DoReb(Team[] teams, int[][] playersOnCourt)
        {

            Random random = new Random();

            int p;
            double[] ratios;

            if (0.15 > random.NextDouble())
            {
                return null;
            }

            if (0.75 * (2 + teams[Defense].CompositeRating.Ratings["GameRebounding"]) / (2 + teams[Offense].CompositeRating.Ratings["GameRebounding"]) > random.NextDouble())
            {
                ratios = RatingArray(playersOnCourt, teams, "GameRebounding", Defense);
                p = playersOnCourt[Defense][ArrayHelper.PickPlayer(ratios)];
                RecordStat(Defense, p, "Drb", playersOnCourt, teams);
                RecordStat(Defense, p, "Trb", playersOnCourt, teams);


                return "Drb";
            }

            ratios = RatingArray(playersOnCourt, teams, "GameRebounding", Offense);
            p = playersOnCourt[Offense][ArrayHelper.PickPlayer(ratios)];
            RecordStat(Offense, p, "Orb", playersOnCourt, teams);
            RecordStat(Defense, p, "Trb", playersOnCourt, teams);


            return "Orb";
        }

        private string DoFg(int shooter, int passer, string type, Team[] teams, int[][] playersOnCourt)
        {
            int p;

            if (passer >= 0)
            {
                p = playersOnCourt[Offense][passer];
                RecordStat(Offense, p, "Ast", playersOnCourt, teams);
            }

            p = playersOnCourt[Offense][shooter];
            RecordStat(Offense, p, "Fg", playersOnCourt, teams);
            RecordStat(Offense, p, "Fga", playersOnCourt, teams);
            RecordStat(Offense, p, "Pts", playersOnCourt, teams, 2);  // 2 points for 2's

            if (type == "AtRim")
            {
                RecordStat(Offense, p, "FgAtRim", playersOnCourt, teams);
                RecordStat(Offense, p, "FgaAtRim", playersOnCourt, teams);
            }
            else if (type == "LowPost")
            {
                RecordStat(Offense, p, "FgLowPost", playersOnCourt, teams);
                RecordStat(Offense, p, "FgaLowPost", playersOnCourt, teams);
            }
            else if (type == "MidRange")
            {
                RecordStat(Offense, p, "FgMidRange", playersOnCourt, teams);
                RecordStat(Offense, p, "FgaMidRange", playersOnCourt, teams);
            }
            else if (type == "ThreePointer")
            {
                RecordStat(Offense, p, "Tp", playersOnCourt, teams);
                RecordStat(Offense, p, "Tpa", playersOnCourt, teams);
                RecordStat(Offense, p, "Pts", playersOnCourt, teams);  // Extra point for 3's
            }

            return "Fg";
        }

        private string DoFt(int shooter, int amount, Team[] teams, int[][] playersOnCourt)
        {
            Random random = new Random();

            int p;
            string outcome = null;

            DoPf(Defense, teams, playersOnCourt);

            p = playersOnCourt[Offense][shooter];

            for (int i = 0; i < amount; i++)
            {
                RecordStat(Offense, p, "Fta", playersOnCourt, teams);

                if (random.NextDouble() < (teams[Offense].Players.Find(player => player.RosterOrder == p).Ratings.LastOrDefault().GameShootingFT * 0.3 + 0.6))  // Between 60% and 90%
                {
                    RecordStat(Offense, p, "Ft", playersOnCourt, teams);
                    RecordStat(Offense, p, "Pts", playersOnCourt, teams);
                    outcome = "Fg";
                }
            }

            if (outcome != "Fg")
            {
                outcome = DoReb(teams, playersOnCourt);  // orb or drb
            }

            return outcome;
        }

        private void DoPf(int t, Team[] teams, int[][] playersOnCourt)
        {
            int p;
            double[] ratios;

            ratios = RatingArray(playersOnCourt, teams, "GameFouling", t);
            p = playersOnCourt[t][ArrayHelper.PickPlayer(ratios)];
            RecordStat(Defense, p, "Pf", playersOnCourt, teams);

            // Foul out
            if (teams[Defense].Players.Find(player => player.RosterOrder == p).Stats.Find(s => s.GameId == Id).Pf >= 6)
            {
                // Force substitutions now
                UpdatePlayersOnCourt(teams, playersOnCourt);
                //UpdateSynergy();
            }
        }




        public void UpdatePlayingTime(int[][] playersOnCourt, Team[] teams)
        {
            int p, t;

            // Time elapsed
            Dt = (Overtimes > 0 ? 5 : 48) / (2 * NumPossessions);



            for (t = 0; t < 2; t++)
            {
                // Update minutes (ovr, court, and bench)
                for (p = 0; p < teams[t].Players.Count; p++)
                {
                    if (playersOnCourt[t].Contains(p))
                    {

                        RecordStat(t, p, "Min", playersOnCourt, teams, Dt);
                        RecordStat(t, p, "CourtTime", playersOnCourt, teams, Dt);
                        RecordStat(t, p, "Energy", playersOnCourt, teams, (int)(-Dt * 0.04 * (1 - teams[t].Players[p].Ratings.LastOrDefault().GameEndurance)));
                        if (teams[t].Players[p].Stats.Find(s => s.GameId == Id).Energy < 0)
                        {
                            teams[t].Players[p].Stats.Find(s => s.GameId == Id).Energy = 0;
                        }
                    }
                    else
                    {
                        RecordStat(t, p, "BenchTime", playersOnCourt, teams, Dt);
                        RecordStat(t, p, "Energy", playersOnCourt, teams, (int)(Dt * 0.1));
                        if (teams[t].Players[p].Stats.Find(s => s.GameId == Id).Energy > 1)
                        {
                            teams[t].Players[p].Stats.Find(s => s.GameId == Id).Energy = 1;
                        }
                    }
                }
            }
        }

        public double[] RatingArray(int[][] playersOnCourt, Team[] teams, string rating, int t, double power = 1)
        {
            double[] array = new double[5];

            for (int i = 0; i < 5; i++)
            {
                int p = playersOnCourt[t][i];
                var player = teams[t].Players.Find(player => player.RosterOrder == p);

                double energy = Fatigue(teams[t].Players[p].Stats.Find(s => s.GameId == Id).Energy);

                if (player != null)
                {
                    var playerRatings = player.Ratings.LastOrDefault();
                    array[i] = Math.Pow(CompositeHelper.GetRatingValue(rating, playerRatings) * energy, power);
                }


            }

            return array;
        }

        public double Fatigue(double energy)
        {
            energy += 0.05;
            if (energy > 1)
            {
                energy = 1;
            }

            return energy;
        }



        public void RecordStat(int t, int p, string s, int[][] playersOnCourt, Team[] teams, int amount = 1)
        {
            amount = amount != 0 ? amount : 1;
            RecordStatHelperPlayer(t, p, s, teams, Id, amount);
            if (s != "Gs" && s != "CourtTime" && s != "BenchTime" && s != "Energy")
            {
                RecordStatHelperTeam(t, p, s, teams, Id, amount);
            }
        }


        public void RecordStatHelperTeam(int t, int p, string s, Team[] teams, int GameId, int amt = 1)
        {

            var team = teams[t];
            if (team != null)
            {
                if (team.Stats == null)
                {
                    team.Stats = new List<TeamGameStats>();
                }

                var lastStats = team.Stats.Find(s => s.GameId == Id);

                if (lastStats == null || lastStats.GameId != GameId)
                {
                    team.Stats.Add(new TeamGameStats { GameId = GameId });
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

        public void RecordStatHelperPlayer(int t, int p, string s, Team[] teams, int GameId, int amt = 1)
        {

            var player = teams[t].Players.Find(player => player.RosterOrder == p);
            if (player != null)
            {
                if (player.Stats == null)
                {
                    player.Stats = new List<PlayerGameStats>();
                }

                var lastStats = player.Stats.Find(s => s.GameId == Id);

                if (lastStats == null || lastStats.GameId != GameId)
                {
                    player.Stats.Add(new PlayerGameStats { GameId = GameId, TeamId = teams[t].Id, Name = player.FullName });
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

        public bool UpdatePlayersOnCourt(Team[] teams, int[][] playersOnCourt)
        {
            bool substitutions = false;

            for (int t = 0; t < 2; t++)
            {
                // Overall ratings scaled by fatigue
                double[] ovrs = new double[teams[t].Players.Count];

                for (int p = 0; p < teams[t].Players.Count; p++)
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
                            player.Stats.Add(new PlayerGameStats { GameId = Id, TeamId = teams[t].Id, Name = player.FullName });
                        }
                    }


                    // Injured or fouled out players can't play
                    if (teams[t].Players[p].Stats.Find(s => s.GameId == Id).Pf >= 6)
                    {
                        ovrs[p] = double.NegativeInfinity;
                    }
                    else
                    {
                        ovrs[p] = teams[t].Players[p].Ratings.LastOrDefault().Ovr * Fatigue(teams[t].Players[p].Stats.Find(s => s.GameId == Id).Energy) *
                            teams[t].Players[p].PtModifier * RandomUniform(0.9, 1.1);
                    }
                }

                // Loop through players on court (in inverse order of current roster position)
                int i = 0;
                for (int pp = 0; pp < playersOnCourt[t].Length; pp++)
                {
                    int p = playersOnCourt[t][pp];
                    playersOnCourt[t][i] = p;

                    // Loop through bench players (in order of current roster position) to see if any should be subbed in
                    for (int b = 0; b < teams[t].Players.Count; b++)
                    {
                        if (!playersOnCourt[t].Contains(b) && ((teams[t].Players[p].Stats.Find(s => s.GameId == Id).CourtTime > 3
                            && teams[t].Players[b].Stats.Find(s => s.GameId == Id).BenchTime > 3
                            && ovrs[b] > ovrs[p]) || teams[t].Players[p].Stats.Find(s => s.GameId == Id).Pf >= 6))
                        {
                            substitutions = true;

                            // Substitute player
                            playersOnCourt[t][i] = b;
                            p = b;
                            teams[t].Players[b].Stats.Find(s => s.GameId == Id).CourtTime = RandomUniform(-2, 2);
                            teams[t].Players[b].Stats.Find(s => s.GameId == Id).BenchTime = RandomUniform(-2, 2);
                            teams[t].Players[p].Stats.Find(s => s.GameId == Id).CourtTime = RandomUniform(-2, 2);
                            teams[t].Players[p].Stats.Find(s => s.GameId == Id).BenchTime = RandomUniform(-2, 2);
                        }
                    }

                    i++;
                }

                if (!StartersRecorded)
                {
                    for (int z = 0; z < 2; z++)
                    {
                        for (int p = 0; p < teams[z].Players.Count; p++)
                        {
                            int playerRosterOrder = teams[z].Players[p].RosterOrder; // Armazena o ID do jogador em uma variável separada

                            if (playersOnCourt[z].Any(play => play == playerRosterOrder))
                            {
                                RecordStat(z, playerRosterOrder, "Gs", playersOnCourt, teams);
                            }
                        }
                    }
                    StartersRecorded = true;
                }

            }

            return substitutions;
        }

        private double RandomUniform(double a, double b)
        {
            return new Random().NextDouble() * (b - a) + a;
        }

        public double Pace(Team team)
        {
            double pace = 0;
            int numPlayers = team.Players.Count;
            if (numPlayers > 7)
            {
                numPlayers = 7;
            }

            for (int i = 0; i < numPlayers; i++)
            {
                pace += team.Players[i].Ratings.LastOrDefault().GamePace;
            }

            pace /= numPlayers;
            return pace * 15 + 100;
        }


    }

}
