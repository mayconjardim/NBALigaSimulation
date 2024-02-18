using NBALigaSimulation.Shared.Engine;
using NBALigaSimulation.Shared.Engine.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using NBALigaSimulation.Shared.Engine.Gameplan;
using NBALigaSimulation.Shared.Engine.Ratings;

namespace NBALigaSimulation.Shared.Models
{
    public class Game
    {
        public int Id { get; set; }
        public int SeasonId { get; set; }
        public Season Season { get; set; }
        public int Type { get; set; }
        public int HomeTeamId { get; set; }
        [ForeignKey("HomeTeamId")]
        public Team? HomeTeam { get; set; }
        public int AwayTeamId { get; set; }
        [ForeignKey("AwayTeamId")]
        public Team AwayTeam { get; set; }
        public bool Happened { get; set; } = false;
        public DateTime GameDate { get; set; }
        public List<TeamGameStats> TeamGameStats { get; set; } = new List<TeamGameStats>();
        public List<PlayerGameStats> PlayerGameStats { get; set; } = new List<PlayerGameStats>();
        public List<GamePlayByPlay> PlayByPlay { get; set; } = new List<GamePlayByPlay>();

        // Atributos Globais da Simulação
        [NotMapped]
        int NumPossessions; // Quantidade posses de uma partida
        [NotMapped]
        bool StartersRecorded = false; // Usado para rastrear se os titulares *reais* foram gravados ou não.
        [NotMapped]
        int SubsEveryN = 6; // Quantas posses esperar antes de fazer substituições
        [NotMapped]
        int Overtimes = 0; // Números de overtimes 
        [NotMapped]
        double SynergyFactor = 0.01; // Qual a importância da sinergia?
        [NotMapped]
        double FatigueFactor = 0.055;
        [NotMapped]
        int Offense; // Time que está atacando
        [NotMapped]
        int Defense; // Time que está defendendo
        [NotMapped]
        double T = 12.00; // Tempo por quarto
        [NotMapped]
        double Dt = 0; // Tempo decorrido por posse
        [NotMapped]
        List<List<int>> PtsQrts = new List<List<int>> { new List<int>(), new List<int>() };

        public void GameSim()
        {

            Team[] Teams = { HomeTeam, AwayTeam };
            CompositeHelper.UpdatePlayersCompositeRating(Teams);

            var paceFactor = 103.1 / 100;
            paceFactor += 0.025 * RandomUtils.Bound((paceFactor - 1) / 0.2, -1, 1);


            NumPossessions = (int)((int)((GameplanUtils.GameplanPace(HomeTeam.Gameplan.Pace) + GameplanUtils.GameplanPace(AwayTeam.Gameplan.Pace)) / 2) * 1.1 * paceFactor);

            Dt = 48.0 / (2 * NumPossessions);

            int[][] PlayersOnCourt = new int[][] { new int[] { 0, 1, 2, 3, 4 }, new int[] { 0, 1, 2, 3, 4 } };

            UpdatePlayersOnCourt(Teams, PlayersOnCourt);
            UpdateSynergy(Teams, PlayersOnCourt);
            HomeCourtAdvantage(Teams, PlayersOnCourt);


            SimPossessions(Teams, PlayersOnCourt);

            // Jogue períodos de prorrogação se necessário
            while (Teams[0].Stats.Find(s => s.GameId == Id)?.Pts == Teams[1].Stats.Find(s => s.GameId == Id)?.Pts)
            {
                if (Overtimes == 0)
                {
                    NumPossessions = (int)Math.Round(NumPossessions * 5.0 / 48); // 5 minutos de posses
                    Dt = 5.0 / (2 * NumPossessions);
                }

                T = 5.0;
                Overtimes++;
                PtsQrts[0].Add(0);
                PtsQrts[1].Add(0);
                //RecordPlay"Overtime", Teams);
                SimPossessions(Teams, PlayersOnCourt);
            }
        }

        public void SimPossessions(Team[] Teams, int[][] PlayersOnCourt)
        {
            int i = 0;
            string outcome;
            bool substitutions;

            Offense = 0;
            Defense = 1;

            while (i < NumPossessions * 2)
            {
                if ((i * Dt > 12 && PtsQrts[0].Count == 1) ||
                    (i * Dt > 24 && PtsQrts[0].Count == 2) ||
                    (i * Dt > 36 && PtsQrts[0].Count == 3))
                {
                    PtsQrts[0].Add(0);
                    PtsQrts[1].Add(0);
                    T = 12;
                    //RecordPlay"Quarter", Teams);
                }

                // Clock
                T -= Dt;
                if (T < 0)
                {
                    T = 0;
                }

                // Troca de posse
                Offense = (Offense == 1) ? 0 : 1;
                Defense = (Offense == 1) ? 0 : 1;

                UpdateTeamCompositeRatings(Teams, PlayersOnCourt);

                outcome = GetPossessionOutcome(Teams, PlayersOnCourt);

                // Troca Offense e Defense para que o receba outra posse quando eles forem trocados novamente no início do loop.
                if (outcome == "Orb")
                {
                    Offense = (Offense == 1) ? 0 : 1;
                    Defense = (Offense == 1) ? 0 : 1;
                }

                UpdatePlayingTime(Teams, PlayersOnCourt);

                //Injuries();

                if (i % SubsEveryN == 0)
                {
                    substitutions = UpdatePlayersOnCourt(Teams, PlayersOnCourt);
                    if (substitutions)
                    {
                        UpdateSynergy(Teams, PlayersOnCourt);
                    }
                }

                i += 1;
            }
        }

        private bool UpdatePlayersOnCourt(Team[] teams, int[][] PlayersOnCourt)
        {
            bool substitutions = false;

            for (int t = 0; t < 2; t++)
            {
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
                            player.Stats.Add(new PlayerGameStats { GameId = Id, TeamId = teams[t].Id, Name = player.Name, Season = Season.Year, Pos = player.Pos });
                        }
                    }

                    if (teams[t].Players[p].Stats.Find(s => s.GameId == Id)?.Pf < 6)
                    {
                        ovrs[p] = teams[t].Players[p].Ratings.LastOrDefault().CalculateOvr * Fatigue(teams[t].Players[p].Stats.Find(s => s.GameId == Id).Energy) *
                              teams[t].Players[p].PtModifier * RandomUtils.RandomUniform(0.9, 1.1);
                    }
                    else
                    {
                        ovrs[p] = double.NegativeInfinity;
                    }
                }
                int i = 0;
                for (int pp = 0; pp < PlayersOnCourt[t].Length; pp++)
                {
                    int p = PlayersOnCourt[t][pp];
                    PlayersOnCourt[t][i] = p;

                    for (int b = 0; b < teams[t].Players.Count; b++)
                    {
                        if (!PlayersOnCourt[t].Contains(b) && ((teams[t].Players[p].Stats.Find(s => s.GameId == Id).CourtTime > 3
                                && teams[t].Players[b].Stats.Find(s => s.GameId == Id).BenchTime > 3
                                && ovrs[b] > ovrs[p]) || teams[t].Players[p].Stats.Find(s => s.GameId == Id).Pf >= 6))
                        {
                            List<string> pos = new List<string>();
                            for (int j = 0; j < PlayersOnCourt[t].Length; j++)
                            {
                                if (j != pp)
                                {
                                    int playerPos = PlayersOnCourt[t][j];

                                    string position = teams[t].Players.Find(player => player.RosterOrder == playerPos).Pos;

                                    pos.Add(position);
                                }
                            }

                            string playersPos2 = teams[t].Players.Find(player => player.RosterOrder == b).Pos;

                            pos.Add(playersPos2);
                            int numG = 0, numPG = 0, numF = 0, numC = 0;
                            foreach (string position in pos)
                            {
                                if (position.Contains("G"))
                                {
                                    numG++;
                                }
                                if (position == "PG")
                                {
                                    numPG++;
                                }
                                if (position.Contains("F"))
                                {
                                    numF++;
                                }
                                if (position == "C")
                                {
                                    numC++;
                                }
                            }
                            if ((numG < 2 && numPG == 0) || (numF < 2 && numC == 0))
                            {
                                if (Fatigue(teams[t].Players[p].Stats.Find(s => s.GameId == Id).Energy) > 0.7)
                                {
                                    continue;
                                }
                            }

                            substitutions = true;

                            // Substitute player
                            PlayersOnCourt[t][i] = b;
                            //p = b;

                            teams[t].Players[b].Stats.Find(s => s.GameId == Id).CourtTime = RandomUtils.RandomUniform(-2, 2);
                            teams[t].Players[b].Stats.Find(s => s.GameId == Id).BenchTime = RandomUtils.RandomUniform(-2, 2);
                            teams[t].Players[p].Stats.Find(s => s.GameId == Id).CourtTime = RandomUtils.RandomUniform(-2, 2);
                            teams[t].Players[p].Stats.Find(s => s.GameId == Id).BenchTime = RandomUtils.RandomUniform(-2, 2);

                            /*
                            if (PlayByPlay != null)
                            {
                                PlayByPlay.Add(new GamePlayByPlay
                                {
                                    Type = "Sub",
                                    T = t,
                                    On = teams[t].Players[b].Id,
                                    Off = teams[t].Players[p].Id
                                });
                            }
                            */

                            if (StartersRecorded)
                            {
                                string[] names = { teams[t].Players[b].Name, teams[t].Players[p].Name };
                                //RecordPlay"Sub", teams, t, names);
                            }
                            break;

                        }
                    }
                    i += 1;
                }
            }

            if (!StartersRecorded)
            {
                for (int z = 0; z < 2; z++)
                {
                    for (int p = 0; p < teams[z].Players.Count; p++)
                    {
                        int playerRosterOrder = teams[z].Players[p].RosterOrder; // Armazena o ID do jogador em uma variável separada

                        if (PlayersOnCourt[z].Any(play => play == playerRosterOrder))
                        {
                            RecordStat(z, playerRosterOrder, "Gs", teams);
                        }
                    }
                }
                StartersRecorded = true;
            }

            return substitutions;
        }

        private void UpdateSynergy(Team[] Teams, int[][] PlayersOnCourt)
        {
            for (int t = 0; t < 2; t++)
            {

                if (Teams[t].Synergy == null)
                {
                    Teams[t].Synergy = new TeamSynergy();
                }

                // Conta todas as habilidades *fracionárias* dos jogadores ativos em uma equipe (incluindo duplicatas)
                Dictionary<string, double> skillsCount = new Dictionary<string, double>
                {
                    { "3", 0 },
                    { "A", 0 },
                    { "B", 0 },
                    { "Di", 0 },
                    { "Dp", 0 },
                    { "Po", 0 },
                    { "Ps", 0 },
                    { "R", 0 }
                };

                for (int i = 0; i < 5; i++)
                {
                    int p = PlayersOnCourt[t][i];

                    skillsCount["3"] += RandomUtils.Sigmoid(Teams[t].Players[p].CompositeRating.Ratings["ShootingThreePointer"], 15, 0.59);
                    skillsCount["B"] += RandomUtils.Sigmoid(Teams[t].Players[p].CompositeRating.Ratings["Dribbling"], 15, 0.68);
                    skillsCount["A"] += RandomUtils.Sigmoid(Teams[t].Players[p].CompositeRating.Ratings["Athleticism"], 15, 0.63);
                    skillsCount["Di"] += RandomUtils.Sigmoid(Teams[t].Players[p].CompositeRating.Ratings["DefenseInterior"], 15, 0.57);
                    skillsCount["Dp"] += RandomUtils.Sigmoid(Teams[t].Players[p].CompositeRating.Ratings["DefensePerimeter"], 15, 0.61);
                    skillsCount["Po"] += RandomUtils.Sigmoid(Teams[t].Players[p].CompositeRating.Ratings["ShootingLowPost"], 15, 0.61);
                    skillsCount["Ps"] += RandomUtils.Sigmoid(Teams[t].Players[p].CompositeRating.Ratings["Passing"], 15, 0.63);
                    skillsCount["R"] += RandomUtils.Sigmoid(Teams[t].Players[p].CompositeRating.Ratings["Rebounding"], 15, 0.61);
                }

                // Sinergia ofensiva de base
                Teams[t].Synergy.Off = 0;
                Teams[t].Synergy.Off += 5 * RandomUtils.Sigmoid(skillsCount["3"], 3, 2);
                Teams[t].Synergy.Off += 3 * RandomUtils.Sigmoid(skillsCount["B"], 15, 0.75) + RandomUtils.Sigmoid(skillsCount["B"], 5, 1.75);
                Teams[t].Synergy.Off += 3 * RandomUtils.Sigmoid(skillsCount["Ps"], 15, 0.75) + RandomUtils.Sigmoid(skillsCount["Ps"], 5, 1.75)
                    + RandomUtils.Sigmoid(skillsCount["Ps"], 5, 2.75);
                Teams[t].Synergy.Off += RandomUtils.Sigmoid(skillsCount["Po"], 15, 0.75);
                Teams[t].Synergy.Off += RandomUtils.Sigmoid(skillsCount["A"], 15, 1.75) + RandomUtils.Sigmoid(skillsCount["A"], 5, 2.75);
                Teams[t].Synergy.Off /= 17;

                // Punir as equipes por não terem múltiplas habilidades de perímetro
                double perimFactor = RandomUtils.Clamp(Math.Sqrt(1 + skillsCount["B"] + skillsCount["Ps"] + skillsCount["3"]) - 1, 0, 2) / 2;
                Teams[t].Synergy.Off *= 0.5 + 0.5 * perimFactor;

                // Sinergia defensiva
                Teams[t].Synergy.Def = 0;
                Teams[t].Synergy.Def += RandomUtils.Sigmoid(skillsCount["Dp"], 15, 0.75);
                Teams[t].Synergy.Def += 2 * RandomUtils.Sigmoid(skillsCount["Di"], 15, 0.75);
                Teams[t].Synergy.Def += RandomUtils.Sigmoid(skillsCount["A"], 5, 2) + RandomUtils.Sigmoid(skillsCount["A"], 5, 3.25);
                Teams[t].Synergy.Def /= 6;

                // Recuperando a sinergia
                Teams[t].Synergy.Reb = 0;
                Teams[t].Synergy.Reb += RandomUtils.Sigmoid(skillsCount["R"], 15, 0.75) + RandomUtils.Sigmoid(skillsCount["R"], 5, 1.75);
                Teams[t].Synergy.Reb /= 4;


            }
        }

        private void HomeCourtAdvantage(Team[] Teams, int[][] PlayersOnCourt)
        {
            double factor;
            for (int t = 0; t < 2; t++)
            {
                if (t == 0)
                {
                    factor = 1.01;  // Bonus pro time da casa
                }
                else
                {
                    factor = 0.99;  // Penalty pro time de fora
                }

                for (int p = 0; p < Teams[t].Players.Count; p++)
                {
                    foreach (string r in Teams[t].Players[p].CompositeRating.Ratings.Keys.ToList())
                    {
                        Teams[t].Players[p].CompositeRating.Ratings[r] *= factor;
                    }
                }
            }
        }

        private void UpdateTeamCompositeRatings(Team[] teams, int[][] playersOnCourt)
        {
            string[] toUpdate = { "GameDribbling", "GamePassing", "GameRebounding", "GameDefense", "GameDefensePerimeter", "GameBlocking", "GamePace" };

            for (int i = 0; i < 2; i++)
            {

                if (teams[i].CompositeRating == null)
                {
                    teams[i].CompositeRating = new TeamCompositeRating();
                }

                for (int j = 0; j < 5; j++)
                {

                    int playerRosterOrder = playersOnCourt[i][j];
                    var playerRatings = teams[i].Players.Find(player => player.RosterOrder == playerRosterOrder).CompositeRating;
                    double ratingValue = 0;
                    string rats = string.Empty;

                    foreach (string rating in toUpdate)
                    {
                        teams[i].CompositeRating.Ratings[rating] = 0;

                        if (rating == "GameDribbling")
                        {
                            ratingValue = playerRatings.Ratings["Dribbling"];
                        }
                        else if (rating == "GamePassing")
                        {
                            ratingValue = playerRatings.Ratings["Passing"];
                        }
                        else if (rating == "GameRebounding")
                        {
                            ratingValue = playerRatings.Ratings["Rebounding"];
                        }
                        else if (rating == "GameDefense")
                        {
                            ratingValue = playerRatings.Ratings["Defense"];
                        }
                        else if (rating == "GameDefensePerimeter")
                        {
                            ratingValue = playerRatings.Ratings["DefensePerimeter"];
                        }
                        else if (rating == "GameBlocking")
                        {
                            ratingValue = playerRatings.Ratings["Blocking"];
                        }

                        teams[i].CompositeRating.Ratings[rating] += ratingValue * Fatigue(teams[i].Players.Find(player => player.RosterOrder == playerRosterOrder).Stats.Find(s => s.GameId == Id).Energy);
                        rats = rating;
                    }

                    teams[i].CompositeRating.Ratings[rats] = teams[i].CompositeRating.Ratings[rats] / 5;

                }

                teams[i].CompositeRating.Ratings["GameDribbling"] += SynergyFactor * teams[i].Synergy.Off;
                teams[i].CompositeRating.Ratings["GamePassing"] += SynergyFactor * teams[i].Synergy.Off;
                teams[i].CompositeRating.Ratings["GameRebounding"] += SynergyFactor * teams[i].Synergy.Reb;
                teams[i].CompositeRating.Ratings["GameDefense"] += SynergyFactor * teams[i].Synergy.Def;
                teams[i].CompositeRating.Ratings["GameDefensePerimeter"] += SynergyFactor * teams[i].Synergy.Def;
                teams[i].CompositeRating.Ratings["GameBlocking"] += SynergyFactor * teams[i].Synergy.Def;
            }
        }

        private void UpdatePlayingTime(Team[] Teams, int[][] PlayersOnCourt)
        {

            for (int t = 0; t < 2; t++)
            {
                for (int p = 0; p < Teams[t].Players.Count; p++)
                {
                    if (PlayersOnCourt[t].Contains(p))
                    {
                        RecordStat(t, p, "Min", Teams, 1, Dt);
                        RecordStat(t, p, "CourtTime", Teams, 1, Dt);
                        // Isso costumava ser 0,04. Aumente mais para diminuir o PT
                        double energyChange = -Dt *
                                   FatigueFactor *
                                   (1 - Teams[t].Players[p].CompositeRating.Ratings["Endurance"]);
                        RecordStat(t, p, "Energy", Teams, 1, energyChange);
                        if (Teams[t].Players[p].Stats.Find(s => s.GameId == Id)?.Energy < 0)
                        {
                            Teams[t].Players[p].Stats.Find(s => s.GameId == Id).Energy = 0;
                        }
                    }
                    else
                    {
                        RecordStat(t, p, "BenchTime", Teams, 1, Dt);
                        RecordStat(t, p, "Energy", Teams, 1, (Dt * 0.1));
                        if (Teams[t].Players[p].Stats.Find(s => s.GameId == Id)?.Energy > 1)
                        {
                            Teams[t].Players[p].Stats.Find(s => s.GameId == Id).Energy = 1;
                        }
                    }
                }
            }
        }

        private string GetPossessionOutcome(Team[] Teams, int[][] PlayersOnCourt)
        {
            if (ProbTov(Teams) > new Random().NextDouble())
            {
                return DoTov(Teams, PlayersOnCourt);
            }

            double[] ratios = RatingArray(Teams, "Usage", Offense, PlayersOnCourt, 10);
            int shooterIndex = PickPlayer(ratios);

            return DoShot(shooterIndex, Teams, PlayersOnCourt);
        }

        private double ProbTov(Team[] Teams)
        {
            double turnoverFactor = 1;
            double defenseRating = 0.14 * Teams[Defense].CompositeRating.Ratings["GameDefense"];
            double dribblingRating = Teams[Offense].CompositeRating.Ratings["GameDribbling"];
            double passingRating = Teams[Offense].CompositeRating.Ratings["GamePassing"];

            double probability = turnoverFactor * (defenseRating) / (0.5 * (dribblingRating + passingRating));

            return BoundProb(probability);
        }

        private string DoTov(Team[] Teams, int[][] PlayersOnCourt)
        {
            double[] ratios = RatingArray(Teams, "Turnovers", Offense, PlayersOnCourt, 2);
            int playerIndex = PickPlayer(ratios);
            var p = PlayersOnCourt[Offense][playerIndex];

            RecordStat(Offense, p, "Tov", Teams);

            if (ProbStl(Teams) > new Random().NextDouble())
            {
                return DoStl(p, Teams, PlayersOnCourt);
            }
            else
            {
                string[] names = { Teams[Offense].Players[p].Name };
                //RecordPlay"Tov", Teams, Offense, names);
            }

            return "Tov";
        }

        private double ProbStl(Team[] Teams)
        {
            double stealFactor = 1.09;
            double defensePerimeter = Teams[Defense].CompositeRating.Ratings["GameDefensePerimeter"];
            double dribbling = Teams[Offense].CompositeRating.Ratings["GameDribbling"];
            double passing = Teams[Offense].CompositeRating.Ratings["GamePassing"];

            double probability = stealFactor * ((0.45 * defensePerimeter) / (0.5 * (dribbling + passing)));

            return BoundProb(probability);
        }

        private string DoStl(int pStoleFrom, Team[] Teams, int[][] PlayersOnCourt)
        {
            double[] ratios = RatingArray(Teams, "Stealing", Defense, PlayersOnCourt, 4);
            int playerIndex = PickPlayer(ratios);
            var p = PlayersOnCourt[Defense][playerIndex];

            RecordStat(Defense, p, "Stl", Teams);

            string[] names = { Teams[Defense].Players[p].Name, Teams[Offense].Players[pStoleFrom].Name, };
            //RecordPlay"Stl", Teams, Defense, names);

            return "Stl";
        }

        private string DoShot(int shooter, Team[] Teams, int[][] PlayersOnCourt)
        {

            int p = PlayersOnCourt[Offense][shooter];
            var player = Teams[Offense].Players.Find(player => player.RosterOrder == p);

            double currentFatigue = Fatigue(Teams[Offense].Players[p].Stats.Find(s => s.GameId == Id).Energy);

            int? passer = null;

            if (ProbAst(Teams) > new Random().NextDouble())
            {
                double[] ratios = RatingArray(Teams, "Passing", Offense, PlayersOnCourt, 10);
                passer = PickPlayer(ratios, shooter);
            }

            double shootingThreePointerScaled = player.CompositeRating.Ratings["ShootingThreePointer"];

            if (shootingThreePointerScaled > 0.55)
            {
                shootingThreePointerScaled = 0.55 + (shootingThreePointerScaled - 0.55) * (0.3 / 0.45);

                if (Teams[Offense].Gameplan.Focus == 3)
                {
                    shootingThreePointerScaled += GameplanUtils.GameplanFocus(Teams[Offense].Gameplan.Focus);
                }

                if (Teams[Offense].Gameplan.Focus == 1)
                {
                    shootingThreePointerScaled += GameplanUtils.GameplanFocus(Teams[Offense].Gameplan.Focus);
                }

            }

            double shootingThreePointerScaled2 = shootingThreePointerScaled;

            if (shootingThreePointerScaled2 < 0.35)
            {
                shootingThreePointerScaled2 = 0 + shootingThreePointerScaled2 * (0.1 / 0.30);
            }
            else if (shootingThreePointerScaled2 < 0.45)
            {
                shootingThreePointerScaled2 = 0.1 + (shootingThreePointerScaled2 - 0.30) * (0.30 / 0.1);
            }

            int? diff = Teams[Defense].Stats.Find(s => s.GameId == Id)?.Pts - Teams[Offense].Stats.Find(s => s.GameId == Id)?.Pts;
            bool forceThreePointer = (diff >= 3 && diff <= 10 && T <= 10.0 / 60.0 && new Random().NextDouble() > T)
                || (T == 0 && Dt <= 2.5 / 60);

            double probAndOne;
            double probMake;
            double probMissAndFoul;
            string type;

            if (forceThreePointer || new Random().NextDouble() < 0.67 * shootingThreePointerScaled2 * 1)
            {
                type = "ThreePointer";
                probMissAndFoul = 0.02;
                probMake = shootingThreePointerScaled * 0.6 + 0.30;
                probAndOne = 0.01;
                probMake *= 1;

            }
            else
            {
                double r1 = 0.8 * new Random().NextDouble() * player.CompositeRating.Ratings["ShootingMidRange"];
                double r2 = new Random().NextDouble() * (player.CompositeRating.Ratings["ShootingAtRim"] + SynergyFactor * (Teams[Offense].Synergy.Off - Teams[Defense].Synergy.Def));
                double r3 = new Random().NextDouble() * (player.CompositeRating.Ratings["ShootingLowPost"] + SynergyFactor * (Teams[Offense].Synergy.Off - Teams[Defense].Synergy.Def));

                if (r1 > r2 && r1 > r3)
                {
                    type = "MidRange";
                    probMissAndFoul = 0.07;
                    probMake = player.CompositeRating.Ratings["ShootingMidRange"] * 0.48 + 0.42;
                    probAndOne = 0.05;
                }
                else if (r2 > r3)
                {
                    type = "AtRim";
                    probMissAndFoul = 0.37;
                    probMake = player.CompositeRating.Ratings["ShootingAtRim"] * 1.58 + 0.54;
                    probAndOne = 0.25;
                }
                else
                {
                    type = "LowPost";
                    probMissAndFoul = 0.33;
                    probMake = player.CompositeRating.Ratings["ShootingLowPost"] * 0.43 + 0.34;
                    probAndOne = 0.15;
                }


                probMake *= 1;
            }

            double foulFactor = 0.65 * (Math.Pow(player.CompositeRating.Ratings["DrawingFouls"] / 0.5, 2)) * 1;

            probMissAndFoul *= foulFactor;
            probAndOne *= foulFactor;
            probMake = (probMake - 0.25 * Teams[Defense].CompositeRating.Ratings["GameDefense"] + SynergyFactor * (Teams[Offense].Synergy.Off - Teams[Defense].Synergy.Def)) * currentFatigue;

            if (T == 0 && Dt < 6.0 / 60)
            {
                probMake *= Math.Sqrt(Dt / (8.0 / 60));
            }

            if (passer != null)
            {
                probMake += 0.025;
            }

            if (ProbBlk(Teams) > new Random().NextDouble())
            {
                return DoBlk(shooter, type, Teams, PlayersOnCourt);
            }

            if (probMake > new Random().NextDouble())
            {
                if (probAndOne > new Random().NextDouble())
                {
                    return DoFg(shooter, passer, type, Teams, PlayersOnCourt, true);
                }

                return DoFg(shooter, passer, type, Teams, PlayersOnCourt);
            }

            if (probMissAndFoul > new Random().NextDouble())
            {
                if (type == "ThreePointer")
                {
                    return DoFt(shooter, 3, Teams, PlayersOnCourt); // fg, orb, or drb
                }
                return DoFt(shooter, 2, Teams, PlayersOnCourt); // fg, orb, or drb
            }

            RecordStat(Offense, p, "Fga", Teams);
            if (type == "AtRim")
            {
                RecordStat(Offense, p, "FgaAtRim", Teams);

                string[] names = { Teams[Offense].Players[p].Name };
                //RecordPlay"MissAtRim", Teams, Offense, names);
            }
            else if (type == "LowPost")
            {
                RecordStat(Offense, p, "FgaLowPost", Teams);

                string[] names = { Teams[Offense].Players[p].Name };
                //RecordPlay"MissLowPost", Teams, Offense, names);
            }
            else if (type == "MidRange")
            {
                RecordStat(Offense, p, "FgaMidRange", Teams);

                string[] names = { Teams[Offense].Players[p].Name };
                //RecordPlay"MissMidRange", Teams, Offense, names);
            }
            else if (type == "ThreePointer")
            {
                RecordStat(Offense, p, "Tpa", Teams);

                string[] names = { Teams[Offense].Players[p].Name };
                //RecordPlay"MissTp", Teams, Offense, names);
            }

            return DoReb(Teams, PlayersOnCourt);

        }

        private double ProbBlk(Team[] Teams)
        {
            return 1 * 0.5 * Math.Pow(Teams[Defense].CompositeRating.Ratings["GameBlocking"], 2);
        }

        private string DoBlk(int shooter, string type, Team[] Teams, int[][] PlayersOnCourt)
        {
            int p = PlayersOnCourt[Offense][shooter];

            double[] blockingRatios = RatingArray(Teams, "Blocking", Defense, PlayersOnCourt, 10);
            int p2 = PlayersOnCourt[Defense][PickPlayer(blockingRatios)];

            RecordStat(Offense, p, "Fga", Teams);

            if (type == "AtRim")
            {
                RecordStat(Offense, p, "FgaAtRim", Teams);

                string[] names = { Teams[Defense].Players[p2].Name, Teams[Offense].Players[p].Name };
                //RecordPlay"BlkAtRim", Teams, Defense, names);
            }
            else if (type == "LowPost")
            {
                RecordStat(Offense, p, "FgaLowPost", Teams);

                string[] names = { Teams[Defense].Players[p2].Name, Teams[Offense].Players[p].Name };
                //RecordPlay"BlkLowPost", Teams, Defense, names);
            }
            else if (type == "MidRange")
            {
                RecordStat(Offense, p, "FgaMidRange", Teams);

                string[] names = { Teams[Defense].Players[p2].Name, Teams[Offense].Players[p].Name };
                //RecordPlay"BlkMidRange", Teams, Defense, names);
            }
            else if (type == "ThreePointer")
            {
                RecordStat(Offense, p, "Tpa", Teams);

                string[] names = { Teams[Defense].Players[p2].Name, Teams[Offense].Players[p].Name };
                //RecordPlay"BlkTp", Teams, Defense, names);
            }

            RecordStat(Defense, p2, "Blk", Teams);

            return DoReb(Teams, PlayersOnCourt);
        }

        private string DoFg(int shooter, int? passer, string type, Team[] Teams, int[][] PlayersOnCourt, bool andOne = false)
        {

            int p = PlayersOnCourt[Offense][shooter];
            RecordStat(Offense, p, "Fga", Teams);
            RecordStat(Offense, p, "Fg", Teams);
            RecordStat(Offense, p, "Pts", Teams, 2);

            if (type == "AtRim")
            {
                RecordStat(Offense, p, "FgaAtRim", Teams);
                RecordStat(Offense, p, "FgAtRim", Teams);

                string[] names = { Teams[Offense].Players[p].Name };
                //RecordPlay"FgAtRim" + (andOne ? "AndOne" : ""), Teams, Offense, names);
            }
            else if (type == "LowPost")
            {
                RecordStat(Offense, p, "FgaLowPost", Teams);
                RecordStat(Offense, p, "FgLowPost", Teams);

                string[] names = { Teams[Offense].Players[p].Name };
                //RecordPlay"FgLowPost" + (andOne ? "AndOne" : ""), Teams, Offense, names);
            }
            else if (type == "MidRange")
            {
                RecordStat(Offense, p, "FgaMidRange", Teams);
                RecordStat(Offense, p, "FgMidRange", Teams);

                string[] names = { Teams[Offense].Players[p].Name };
                //RecordPlay"FgMidRange" + (andOne ? "AndOne" : ""), Teams, Offense, names);

            }
            else if (type == "ThreePointer")
            {
                RecordStat(Offense, p, "Pts", Teams);
                RecordStat(Offense, p, "Tpa", Teams);
                RecordStat(Offense, p, "Tp", Teams);

                string[] names = { Teams[Offense].Players[p].Name };
                //RecordPlay"Tp" + (andOne ? "AndOne" : ""), Teams, Offense, names);

            }

            if (passer.HasValue)
            {
                int p2 = PlayersOnCourt[Offense][passer.Value];
                RecordStat(Offense, p2, "Ast", Teams);

                string[] names = { Teams[Offense].Players[p2].Name };
                //RecordPlay"Ast", Teams, Offense, names);
            }

            if (andOne)
            {
                return DoFt(shooter, 1, Teams, PlayersOnCourt);
            }

            return "Fg";
        }

        private double ProbAst(Team[] Teams)
        {
            double numerator = (0.9 * (2 + Teams[Offense].CompositeRating.Ratings["GamePassing"]) + GameplanUtils.GameplanMotion(Teams[Offense].Gameplan.Pace));
            double denominator = (2 + Teams[Defense].CompositeRating.Ratings["GameDefense"]);

            return (numerator / denominator) * 1;
        }


        private string DoFt(int shooter, int amount, Team[] Teams, int[][] PlayersOnCourt)
        {
            DoPf(Defense, Teams, PlayersOnCourt);
            int p = PlayersOnCourt[Offense][shooter];

            var player = Teams[Offense].Players.Find(player => player.RosterOrder == p);
            double ftp = RandomUtils.Bound(player.CompositeRating.Ratings["ShootingFT"] * 0.6 + 0.45, 0, 0.95);

            string outcome = null;
            for (int i = 0; i < amount; i++)
            {
                RecordStat(Offense, p, "Fta", Teams);
                if (new Random().NextDouble() < ftp)
                {
                    RecordStat(Offense, p, "Ft", Teams);
                    RecordStat(Offense, p, "Pts", Teams);

                    string[] names = { Teams[Offense].Players[p].Name };
                    //RecordPlay"Ft", Teams, Offense, names);

                    outcome = "Fg";
                }
                else
                {
                    string[] names = { Teams[Offense].Players[p].Name };
                    //RecordPlay"MissFt", Teams, Offense, names);
                    outcome = null;
                }
            }

            if (outcome != "Fg")
            {
                outcome = DoReb(Teams, PlayersOnCourt);
            }

            return outcome;
        }

        private void DoPf(int t, Team[] Teams, int[][] PlayersOnCourt)
        {

            double[] ratios = RatingArray(Teams, "Fouling", t, PlayersOnCourt, 2);
            int p = PlayersOnCourt[t][PickPlayer(ratios)];
            RecordStat(Defense, p, "Pf", Teams);


            string[] names = { Teams[Defense].Players[p].Name };
            //RecordPlay"Pf", Teams, Defense, names);

            var player = Teams[Defense].Players.Find(player => player.RosterOrder == p);

            if (player.Stats.Find(s => s.GameId == Id).Pf >= 6)
            {

                //RecordPlay"FoulOut", Teams, Defense, names);

                UpdatePlayersOnCourt(Teams, PlayersOnCourt);
                UpdateSynergy(Teams, PlayersOnCourt);
            }
        }

        public string DoReb(Team[] Teams, int[][] PlayersOnCourt)
        {
            int p;
            double[] ratios;

            if (new Random().NextDouble() < 0.15)
            {
                return null;
            }

            double defensiveReboundChance = (0.75 * (2 + Teams[Defense].CompositeRating.Ratings["GameRebounding"])) /
                (1 * (2 + Teams[Offense].CompositeRating.Ratings["GameRebounding"]));

            if (defensiveReboundChance > new Random().NextDouble())
            {
                ratios = RatingArray(Teams, "Rebounding", Defense, PlayersOnCourt, 10);
                p = PlayersOnCourt[Defense][PickPlayer(ratios)];
                RecordStat(Defense, p, "Drb", Teams);
                RecordStat(Defense, p, "Trb", Teams);

                string[] name = { Teams[Defense].Players[p].Name };
                //RecordPlay"Drb", Teams, Defense, name);

                return "Drb";
            }

            ratios = RatingArray(Teams, "Rebounding", Offense, PlayersOnCourt, 5);
            int oP = PlayersOnCourt[Offense][PickPlayer(ratios)];
            RecordStat(Offense, oP, "Orb", Teams);
            RecordStat(Offense, oP, "Trb", Teams);

            string[] names = { Teams[Offense].Players[oP].Name };
            //RecordPlay"Orb", Teams, Defense, names);

            return "Orb";
        }

        private double[] RatingArray(Team[] Teams, string rating, int t, int[][] PlayersOnCourt, double power = 1)
        {
            double[] array = new double[5];
            double total = 0;


            for (int i = 0; i < 5; i++)
            {
                int p = PlayersOnCourt[t][i];
                double compositeRating = Teams[t].Players[p].CompositeRating.Ratings[rating];

                if (rating == "Fouling")
                {
                    int pf = Teams[t].Players[p].Stats.Find(s => s.GameId == Id).Pf;
                    if (pf == 6 - 1)
                    {
                        compositeRating *= 0.8;
                    }
                    else if (pf == 6)
                    {
                        compositeRating *= 0.5;
                    }
                    else if (pf > 6)
                    {
                        compositeRating *= 0.25;
                    }
                }

                array[i] = Math.Pow(compositeRating * Fatigue(Teams[t].Players[p].Stats.Find(s => s.GameId == Id).Energy), power);
                total += array[i];
            }

            double floor = 0.05 * total;

            for (int i = 0; i < 5; i++)
            {
                if (array[i] < floor)
                {
                    array[i] = floor;
                }
            }

            return array;
        }


        private void RecordStat(int t, int p, string s, Team[] teams, int amount = 1, double amntDouble = 1.0)
        {
            amount = amount != 0 ? amount : 1;
            RecordHelper.RecordStatHelperPlayer(GameDate, t, p, s, Id, teams, Type, Season.Year, amount, amntDouble);
            if (s != "Gs" && s != "CourtTime" && s != "BenchTime" && s != "Energy")
            {
                RecordHelper.RecordStatHelperTeam(t, p, s, Id, teams, Season.Year, amount);
                if (s == "Pts")
                {
                    int length = PtsQrts[t].Count;
                    if (length > 0)
                    {
                        PtsQrts[t][length - 1] += amount;
                    }
                    else
                    {
                        PtsQrts[t].Add(amount);
                    }
                }

            }
        }

        private double Fatigue(double energy)
        {
            energy += 0.05;
            if (energy > 1)
            {
                energy = 1;
            }

            return energy;
        }

        private int PickPlayer(double[] ratios, int? exempt = null)
        {
            if (exempt.HasValue)
            {
                ratios[exempt.Value] = 0;
            }

            double sum = ratios.Sum();

            if (sum == 0)
            {
                List<int> candidates = Enumerable.Range(0, ratios.Length).Where(i => i != exempt).ToList();
                Random random = new Random();
                return candidates[random.Next(candidates.Count)];
            }

            double rand = new Random().NextDouble() * sum;

            double runningSum = 0;

            for (int i = 0; i < ratios.Length; i++)
            {
                runningSum += ratios[i];
                if (rand < runningSum)
                {
                    return i;
                }
            }

            return 0;
        }

        private double BoundProb(double prob)
        {
            double boundedProb = RandomUtils.Bound(prob, 0.001, 0.999);
            return boundedProb;
        }

        public void RecordPlay(string type, Team[] teams, int? t = null, string[]? names = null)
        {

            string[] texts = null;
            string text;
            string teamName = string.Empty;
            string score = string.Empty;
            int i;
            double sec;

            if (PlayByPlay == null)
            {
                PlayByPlay = new List<GamePlayByPlay>();
            }

            if (PlayByPlay != null)
            {
                switch (type)
                {
                    case "Injury":
                        texts = new string[] { "{0} was injured!" };
                        break;
                    case "Tov":
                        texts = new string[] { "{0} turned the ball over" };
                        break;
                    case "Stl":
                        texts = new string[] { "{0} stole the ball from {1}" };
                        break;
                    case "FgAtRim":
                        texts = new string[] { "{0} made a dunk/layup" };
                        break;
                    case "FgAtRimAndOne":
                        texts = new string[] { "{0} made a dunk/layup and got fouled!" };
                        break;
                    case "FgLowPost":
                        texts = new string[] { "{0} made a low post shot" };
                        break;
                    case "FgLowPostAndOne":
                        texts = new string[] { "{0} made a low post shot and got fouled!" };
                        break;
                    case "FgMidRange":
                        texts = new string[] { "{0} made a mid-range shot" };
                        break;
                    case "FgMidRangeAndOne":
                        texts = new string[] { "{0} made a mid-range shot and got fouled!" };
                        break;
                    case "Tp":
                        texts = new string[] { "{0} made a three-pointer shot" };
                        break;
                    case "TpAndOne":
                        texts = new string[] { "{0} made a three-pointer and got fouled!" };
                        break;
                    case "BlkAtRim":
                        texts = new string[] { "{0} blocked {1}'s dunk/layup" };
                        break;
                    case "BlkLowPost":
                        texts = new string[] { "{0} blocked {1}'s low post shot" };
                        break;
                    case "BlkMidRange":
                        texts = new string[] { "{0} blocked {1}'s mid-range shot" };
                        break;
                    case "BlkTp":
                        texts = new string[] { "{0} blocked {1}'s three-pointer" };
                        break;
                    case "MissAtRim":
                        texts = new string[] { "{0} missed a dunk/layup" };
                        break;
                    case "MissLowPost":
                        texts = new string[] { "{0} missed a low post shot" };
                        break;
                    case "MissMidRange":
                        texts = new string[] { "{0} missed a mid-range shot" };
                        break;
                    case "MissTp":
                        texts = new string[] { "{0} missed a three-pointer" };
                        break;
                    case "Orb":
                        texts = new string[] { "{0} grabbed the offensive rebound" };
                        break;
                    case "Drb":
                        texts = new string[] { "{0} grabbed the defensive rebound" };
                        break;
                    case "Ast":
                        texts = new string[] { "(assist: {0})" };
                        break;
                    case "Quarter":
                        texts = new string[] { "Start of " + PbpHelper.Ordinal(PtsQrts[0].Count) + " quarter" };
                        break;
                    case "Overtime":
                        texts = new string[] { "Start of " + PbpHelper.Ordinal(PtsQrts[1].Count - 4) + " overtime period" };
                        break;
                    case "Ft":
                        texts = new string[] { "{0} made a free throw" };
                        break;
                    case "MissFt":
                        texts = new string[] { "{0} missed a free throw" };
                        break;
                    case "Pf":
                        texts = new string[] { "Foul on {0}" };
                        break;
                    case "FoulOut":
                        texts = new string[] { "{0} fouled out" };
                        break;
                    case "Sub":
                        texts = new string[] { "Substitution: {0} for {1}" };
                        break;
                }

                if (t != null)
                {
                    int team = (int)t;
                    teamName = teams[team].Abrv;
                    score = (teams[0].Abrv + " " + teams[0].Stats.Find(s => s.GameId == Id)?.Pts + " - " + teams[1].Abrv + " " + teams[1].Stats.Find(s => s.GameId == Id)?.Pts);
                }

                if (texts != null)
                {
                    text = texts[0];
                    if (names != null)
                    {
                        for (i = 0; i < names.Length; i++)
                        {
                            text = text.Replace("{" + i + "}", names[i]);
                        }
                    }

                    if (type == "Ast")
                    {
                        for (i = PlayByPlay.Count - 1; i >= 0; i--)
                        {
                            if (PlayByPlay[i].Type == "text")
                            {
                                PlayByPlay[i].Text += " " + text;
                                break;
                            }
                        }
                    }
                    else
                    {
                        sec = (int)Math.Floor((T * 60) % 60); // Converter minutos em segundos
                        string secs = sec < 10 ? "0" + sec.ToString() : sec.ToString();
                        PlayByPlay.Add(new GamePlayByPlay
                        {
                            Type = "text",
                            Text = text,
                            T = T,
                            Time = Math.Floor(T) + ":" + secs,
                            On = 0,
                            Off = 0,
                            TeamName = teamName,
                            Score = score
                        });
                    }
                }
                else
                {
                    Console.WriteLine("No text for " + type);
                }
            }

        }

    }

}