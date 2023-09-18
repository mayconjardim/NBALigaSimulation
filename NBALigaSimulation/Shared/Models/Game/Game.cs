using NBALigaSimulation.Shared.Engine;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

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
        int Offense; // Time que está atacando
        [NotMapped]
        int Defense; // Time que está defendendo
        [NotMapped]
        double T = 12.00; // Tempo por quarto
        [NotMapped]
        double Dt = 0; // Tempo decorrido por posse
        [NotMapped]
        List<List<int>> PtsQtrs = new List<List<int>>();

        public void GameSim()
        {

            Team[] Teams = { HomeTeam, AwayTeam };
            CompositeHelper.UpdatePlayersCompositeRating(Teams);
            CompositeHelper.UpdatePace(Teams);

            NumPossessions = (int)Math.Round((Teams[0].CompositeRating.Ratings["GamePace"] +
                Teams[1].CompositeRating.Ratings["GamePace"]) / 2 * RandomUtils.RandomUniform(0.9, 1.1));

            Dt = 48.0 / (2 * NumPossessions);

            int[][] PlayersOnCourt = new int[][] { new int[] { 0, 1, 2, 3, 4 }, new int[] { 0, 1, 2, 3, 4 } };

            UpdatePlayersOnCourt(Teams, PlayersOnCourt);
            UpdateSynergy(Teams, PlayersOnCourt);
            HomeCourtAdvantage(Teams, PlayersOnCourt);


            SimRegulation(Teams, PlayersOnCourt);

            // Jogue períodos de prorrogação se necessário
            while (Teams[0].Stats.Find(s => s.GameId == Id)?.Pts == Teams[1].Stats.Find(s => s.GameId == Id)?.Pts)
            {
                SimOvertime(Teams, PlayersOnCourt);
            }

        }

        private void SimRegulation(Team[] Teams, int[][] PlayersOnCourt)
        {
            Offense = 0;
            Defense = 1;
            int quarter = 1;

            while (true)
            {
                while (T > 0)
                {
                    SimPossession(Teams, PlayersOnCourt);
                }
                quarter += 1;

                if (quarter == 5)
                {
                    break;
                }
            }
        }

        private void SimOvertime(Team[] Teams, int[][] PlayersOnCourt)
        {
            T = (int)Math.Ceiling(0.4 * 5);
            Overtimes += 1;

            Offense = (new Random().NextDouble() < 0.5) ? 0 : 1;
            Defense = (Offense == 0) ? 1 : 0;

            while (T > 0)
            {
                SimPossession(Teams, PlayersOnCourt);
            }
        }

        private void SimPossession(Team[] Teams, int[][] PlayersOnCourt)
        {
            T -= Dt;
            double possessionTime = Dt;

            if (T < 0)
            {
                possessionTime += T;
                T = 0;
            }

            Offense = (Offense == 1) ? 0 : 1;
            Defense = (Offense == 1) ? 0 : 1;

            UpdateTeamCompositeRatings(Teams, PlayersOnCourt);

            string outcome = GetPossessionOutcome(Teams, PlayersOnCourt);

            if (outcome == "Orb")
            {
                Offense = (Offense == 1) ? 0 : 1;
                Defense = (Offense == 1) ? 0 : 1;
            }

            UpdatePlayingTime(Teams, PlayersOnCourt, possessionTime);

            //Injuries();

            if (RandomUtils.RandInt(1, SubsEveryN + 1) == 1)
            {
                bool substitutions = UpdatePlayersOnCourt(Teams, PlayersOnCourt);

                if (substitutions)
                {
                    UpdateSynergy(Teams, PlayersOnCourt);
                }
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
                            player.Stats.Add(new PlayerGameStats { GameId = Id, TeamId = teams[t].Id, Name = player.Name, Season = Season.Year });
                        }
                    }

                    if (teams[t].Players[p].Stats.Find(s => s.GameId == Id).Pf >= 6)
                    {
                        ovrs[p] = double.NegativeInfinity;
                    }
                    else
                    {
                        ovrs[p] = teams[t].Players[p].Ratings.LastOrDefault().CalculateOvr * Fatigue(teams[t].Players[p].Stats.Find(s => s.GameId == Id).Energy) *
                              teams[t].Players[p].PtModifier * RandomUtils.RandomUniform(0.9, 1.1);
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
                            p = b;

                            teams[t].Players[b].Stats.Find(s => s.GameId == Id).CourtTime = RandomUtils.RandomUniform(-2, 2);
                            teams[t].Players[b].Stats.Find(s => s.GameId == Id).BenchTime = RandomUtils.RandomUniform(-2, 2);
                            teams[t].Players[p].Stats.Find(s => s.GameId == Id).CourtTime = RandomUtils.RandomUniform(-2, 2);
                            teams[t].Players[p].Stats.Find(s => s.GameId == Id).BenchTime = RandomUtils.RandomUniform(-2, 2);

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

                    skillsCount["3"] += RandomUtils.Sigmoid(Teams[t].Players[p].CompositeRating.Ratings["ShootingThreePointer"], 15, 0.7);
                    skillsCount["B"] += RandomUtils.Sigmoid(Teams[t].Players[p].CompositeRating.Ratings["Dribbling"], 15, 0.7);
                    skillsCount["A"] += RandomUtils.Sigmoid(Teams[t].Players[p].CompositeRating.Ratings["Athleticism"], 15, 0.7);
                    skillsCount["Di"] += RandomUtils.Sigmoid(Teams[t].Players[p].CompositeRating.Ratings["DefenseInterior"], 15, 0.7);
                    skillsCount["Dp"] += RandomUtils.Sigmoid(Teams[t].Players[p].CompositeRating.Ratings["DefensePerimeter"], 15, 0.7);
                    skillsCount["Po"] += RandomUtils.Sigmoid(Teams[t].Players[p].CompositeRating.Ratings["ShootingLowPost"], 15, 0.7);
                    skillsCount["Ps"] += RandomUtils.Sigmoid(Teams[t].Players[p].CompositeRating.Ratings["Passing"], 15, 0.7);
                    skillsCount["R"] += RandomUtils.Sigmoid(Teams[t].Players[p].CompositeRating.Ratings["Rebounding"], 15, 0.7);
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

        private void UpdatePlayingTime(Team[] Teams, int[][] PlayersOnCourt, double possessionTime)
        {

            for (int t = 0; t < 2; t++)
            {
                for (int p = 0; p < Teams[t].Players.Count; p++)
                {
                    if (PlayersOnCourt[t].Contains(p))
                    {
                        RecordStat(t, p, "Min", Teams, 1, possessionTime);
                        RecordStat(t, p, "CourtTime", Teams, 1, possessionTime);
                        // Isso costumava ser 0,04. Aumente mais para diminuir o PT
                        RecordStat(t, p, "Energy", Teams, 1, (-possessionTime * 0.06 * (1 - Teams[t].Players[p].CompositeRating.Ratings["Endurance"])));
                        if (Teams[t].Players[p].Stats.Find(s => s.GameId == Id)?.Energy < 0)
                        {
                            Teams[t].Players[p].Stats.Find(s => s.GameId == Id).Energy = 0;
                        }
                    }
                    else
                    {
                        RecordStat(t, p, "BenchTime", Teams, 1, possessionTime);
                        RecordStat(t, p, "Energy", Teams, 1, (possessionTime * 0.1));
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

            double[] ratios = RatingArray("Usage", Offense);
            int shooterIndex = PickPlayer(ratios);

            return DoShot(shooterIndex, Teams, PlayersOnCourt);
        }

        private double ProbTov(Team[] Teams)
        {

            double defenseRating = Teams[Defense].CompositeRating.Ratings["GameDefense"];
            double offenseRating = 0.5 * (Teams[Offense].CompositeRating.Ratings["GameDribbling"] + Teams[Offense].CompositeRating.Ratings["GamePassing"]);

            return (0.14 * defenseRating) / offenseRating;
        }

        private string DoTov(Team[] Teams, int[][] PlayersOnCourt)
        {
            double[] ratios = RatingArray(Teams, "GameTurnovers", Offense, PlayersOnCourt, 2);
            int playerIndex = PickPlayer(ratios);
            var p = PlayersOnCourt[Offense][playerIndex];

            RecordStat(Offense, p, "Tov", Teams);

            if (ProbStl(Teams) > new Random().NextDouble())
            {
                return DoStl(p);
            }

            return "Tov";
        }

        private double ProbStl(Team[] Teams)
        {
            double defensePerimeterRating = Teams[Defense].CompositeRating.Ratings["GameDefensePerimeter"];
            double offenseRating = 0.5 * (Teams[Offense].CompositeRating.Ratings["GameDribbling"] + Teams[Offense].CompositeRating.Ratings["GamePassing"]);

            return (0.55 * defensePerimeterRating) / offenseRating;
        }

        private string DoStl(int pStoleFrom, Team[] Teams, int[][] PlayersOnCourt)
        {
            double[] ratios = RatingArray(Teams, "GameStealing", Defense, PlayersOnCourt, 5);
            int playerIndex = PickPlayer(ratios);
            var p = PlayersOnCourt[Defense][playerIndex];

            RecordStat(Defense, p, "Stl", Teams);

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
                double[] passingRatios = RatingArray("GamePassing", Offense, PlayersOnCourt, 10);
                passer = PickPlayer(passingRatios, shooter);
            }

            double shootingThreePointerScaled = player.CompositeRating.Ratings["ShootingThreePointer"];
            if (shootingThreePointerScaled > 0.55)
            {
                shootingThreePointerScaled = 0.55 + (shootingThreePointerScaled - 0.55) * (0.3 / 0.45);
            }

            double probAndOne;
            double probMake;
            double probMissAndFoul;
            string type;
            if (player.CompositeRating.Ratings["ShootingThreePointer"] > 0.35
                && new Random().NextDouble() < 0.67 * shootingThreePointerScaled)
            {
                type = "ThreePointer";
                probMissAndFoul = 0.02;
                probMake = shootingThreePointerScaled * 0.3 + 0.36;
                probAndOne = 0.01;
            }
            else
            {
                double r1 = 0.8 * new Random().NextDouble() * player.CompositeRating.Ratings["ShootingMidRange"];
                double r2 = new Random().NextDouble() *
                            (player.CompositeRating.Ratings["ShootingAtRim"] +
                            SynergyFactor *
                            (Teams[Offense].Synergy.Off - Teams[Defense].Synergy.Def));
                double r3 = new Random().NextDouble() *
                            (player.CompositeRating.Ratings["ShootingLowPost"] +
                            SynergyFactor *
                            (Teams[Offense].Synergy.Off - Teams[Defense].Synergy.Def));
                if (r1 > r2 && r1 > r3)
                {
                    type = "MidRange";
                    probMissAndFoul = 0.07;
                    probMake = player.CompositeRating.Ratings["ShootingMidRange"] * 0.32 + 0.32;
                    probAndOne = 0.05;
                }
                else if (r2 > r3)
                {
                    type = "AtRim";
                    probMissAndFoul = 0.37;
                    probMake = player.CompositeRating.Ratings["ShootingAtRim"] * 0.32 + 0.52;
                    probAndOne = 0.25;
                }
                else
                {
                    type = "LowPost";
                    probMissAndFoul = 0.33;
                    probMake = player.CompositeRating.Ratings["ShootingLowPost"] * 0.32 + 0.37;
                    probAndOne = 0.15;
                }
            }

            double foulFactor = 0.8 * Math.Pow(player.CompositeRating.Ratings["DrawingFouls"] / 0.5, 2);
            probMissAndFoul *= foulFactor;
            probAndOne *= foulFactor;

            probMake = (probMake - 0.25 * Teams[Defense].CompositeRating.Ratings["GameDefense"] +
                        SynergyFactor *
                        (Teams[Offense].Synergy.Off - Teams[Defense].Synergy.Def)) * currentFatigue;

            if (passer.HasValue)
            {
                probMake += 0.025;
            }

            if (ProbBlk(Teams) > new Random().NextDouble())
            {
                return DoBlk(shooter, type); // orb or drb
            }

            if (probMake > new Random().NextDouble())
            {
                if (probAndOne > new Random().NextDouble())
                {
                    return DoFg(shooter, passer, type, true); // fg, orb, or drb
                }
                return DoFg(shooter, passer, type); // fg
            }

            if (probMissAndFoul > new Random().NextDouble())
            {
                if (type == "threePointer")
                {
                    return DoFt(shooter, 3); // fg, orb, or drb
                }
                return DoFt(shooter, 2); // fg, orb, or drb
            }

            RecordStat(Offense, p, "Fga", Teams);
            if (type == "AtRim")
            {
                RecordStat(Offense, p, "FgaAtRim", Teams);
            }
            else if (type == "LowPost")
            {
                RecordStat(Offense, p, "FgaLowPost", Teams);
            }
            else if (type == "MidRange")
            {
                RecordStat(Offense, p, "FgaMidRange", Teams);
            }
            else if (type == "ThreePointer")
            {
                RecordStat(Offense, p, "Tpa", Teams);
            }

            return DoReb(Teams, PlayersOnCourt);
        }

        private double ProbBlk(Team[] Teams)
        {
            double blockingRating = Teams[Defense].CompositeRating.Ratings["GameBlocking"];
            return 0.2 * Math.Pow(blockingRating, 2);
        }

        private string DoBlk(int shooter, string type, Team[] Teams, int[][] PlayersOnCourt)
        {
            int p = PlayersOnCourt[Offense][shooter];
            RecordStat(Offense, p, "ba");
            RecordStat(Offense, p, "fga");

            if (type == "AtRim")
            {
                RecordStat(Offense, p, "FgaAtRim", Teams);
            }
            else if (type == "LowPost")
            {
                RecordStat(Offense, p, "FgaLowPost", Teams);
            }
            else if (type == "MidRange")
            {
                RecordStat(Offense, p, "FgaMidRange", Teams);
            }
            else if (type == "ThreePointer")
            {
                RecordStat(Offense, p, "Tpa", Teams);
            }

            double[] blockingRatios = RatingArray(Teams, "GameBlocking", Defense, PlayersOnCourt, 10);
            int p2 = PlayersOnCourt[Defense][PickPlayer(blockingRatios)];
            RecordStat(Defense, p, "Blk", Teams);

            return DoReb(Teams, PlayersOnCourt);
        }

        private string DoFg(int shooter, int? passer, string type, bool andOne = false, Team[] Teams, int[][] PlayersOnCourt)
        {

            int p = PlayersOnCourt[Offense][shooter];
            RecordStat(Offense, p, "Fga", Teams);
            RecordStat(Offense, p, "Fg", Teams);
            RecordStat(Offense, p, "Pts", Teams, 2);

            if (type == "AtRim")
            {
                RecordStat(Offense, p, "FgaAtRim", Teams);
                RecordStat(Offense, p, "FgAtRim", Teams);

            }
            else if (type == "LowPost")
            {
                RecordStat(Offense, p, "FgaLowPost", Teams);
                RecordStat(Offense, p, "FgLowPost");

            }
            else if (type == "MidRange")
            {
                RecordStat(Offense, p, "FgaMidRange", Teams);
                RecordStat(Offense, p, "FgMidRange", Teams);

            }
            else if (type == "ThreePointer")
            {
                RecordStat(Offense, p, "Pts", Teams);
                RecordStat(Offense, p, "Tpa", Teams);
                RecordStat(Offense, p, "Tp", Teams);

            }

            if (passer.HasValue)
            {
                int p2 = PlayersOnCourt[Offense][passer.Value];
                RecordStat(Offense, p2, "Ast", Teams);
            }

            if (andOne)
            {
                return DoFt(shooter, 1, Teams, PlayersOnCourt);
            }

            return "Fg";
        }

        private double ProbAst(Team[] Teams)
        {
            double passingRating = Teams[Offense].CompositeRating.Ratings["GamePassing"];
            double defenseRating = Teams[Defense].CompositeRating.Ratings["GameDefense"];

            return (0.6 * (2 + passingRating)) / (2 + defenseRating);
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
                    outcome = "Fg";
                }
                else
                {
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

            double[] ratios = RatingArray(Teams, "GameFouling", t, PlayersOnCourt, 2);
            int p = PlayersOnCourt[t][PickPlayer(ratios)];
            RecordStat(Defense, p, "Pf", Teams);

            var player = Teams[Defense].Players.Find(player => player.RosterOrder == p);

            if (player.Stats.Find(s => s.GameId == Id).Pf >= 6)
            {
                UpdatePlayersOnCourt(Teams, PlayersOnCourt);
                UpdateSynergy(Teams, PlayersOnCourt);
            }
        }

        private string DoReb(Team[] Teams, int[][] PlayersOnCourt)
        {
            double randomValue = new Random().NextDouble();

            if (randomValue < 0.15)
            {
                return null;
            }

            double defenseReboundingRatio = (0.75 * (2 + Teams[Defense].CompositeRating.Ratings["GameRebounding"])) /
                                            (2 + Teams[Offense].CompositeRating.Ratings["GameRebounding"]);

            double[] ratios;
            int p;

            if (defenseReboundingRatio > new Random().NextDouble())
            {
                ratios = RatingArray(Teams, "Rebounding", Defense, 2);
                p = PlayersOnCourt[Defense][PickPlayer(ratios)];
                RecordStat(Defense, p, "Drb", Teams);
                return "Drb";
            }

            ratios = RatingArray(Teams, "GameRebounding", Offense, PlayersOnCourt, 3);
            p = PlayersOnCourt[Offense][PickPlayer(ratios)];
            RecordStat(Offense, p, "Orb");

            return "Orb";
        }


    }

}