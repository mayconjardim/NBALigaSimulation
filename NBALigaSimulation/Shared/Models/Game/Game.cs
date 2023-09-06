using NBALigaSimulation.Shared.Engine;
using System.ComponentModel.DataAnnotations.Schema;

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

        // Atributos Globais da Simulação de um jogo
        [NotMapped]
        int NumPossessions; // Quantidade posses de uma partida
        [NotMapped]
        bool StartersRecorded = false; // Usado para rastrear se os titulares *reais* foram gravados ou não.
        [NotMapped]
        int SubsEveryN = 6; // Quantas posses esperar antes de fazer substituições
        [NotMapped]
        int Overtimes = 0; // Números de overtimes 
        [NotMapped]
        decimal SynergyFactor = 0.01m; // Qual a importância da sinergia?
        [NotMapped]
        int Offense; // Time que está atacando
        [NotMapped]
        int Defense; // Time que está defendendo
        [NotMapped]
        double T = 12.00; // Tempo por quarto
        [NotMapped]
        decimal Dt = 0; // Tempo decorrido por posse
        [NotMapped]
        decimal PaceFactor = 99.2m / 100;
        [NotMapped]
        List<List<int>> PtsQtrs = new List<List<int>>();

        public void GameSim()
        {

            Team[] Teams = { HomeTeam, AwayTeam };
            CompositeHelper.UpdatePlayersCompositeRating(Teams);
            CompositeHelper.UpdatePace(Teams);


            PaceFactor += 0.025m * RandomUtils.Bound((PaceFactor - 1) / 0.2m, -1, 1);
            NumPossessions = (int)Math.Round((Teams[0].CompositeRating.Ratings["GamePace"] + Teams[1].CompositeRating.Ratings["GamePace"] / 2) * 1.1m * PaceFactor);

            Dt = 48.0m / (2 * NumPossessions);

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
                    Dt = 5.0m / (2 * NumPossessions);
                }

                T = 5.0;
                Overtimes++;
                SimPossessions(Teams, PlayersOnCourt);
            }

        }

        private void HomeCourtAdvantage(Team[] Teams, int[][] PlayersOnCourt)
        {
            decimal factor;
            for (int t = 0; t < 2; t++)
            {
                if (t == 0)
                {
                    factor = 1.01m;  // Bonus pro time da casa
                }
                else
                {
                    factor = 0.99m;  // Penalty pro time de fora
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

        private void SimPossessions(Team[] Teams, int[][] PlayersOnCourt)
        {
            int i;
            string outcome;
            bool substitutions;

            Offense = 0;
            Defense = 1;

            i = 0;
            while (i < this.NumPossessions * 2)
            {
                // Troca de posse
                Offense = (Offense == 1) ? 0 : 1;
                Defense = (Offense == 1) ? 0 : 1;

                if (i % SubsEveryN == 0)
                {
                    substitutions = UpdatePlayersOnCourt(Teams, PlayersOnCourt);
                    if (substitutions)
                    {
                        UpdateSynergy(Teams, PlayersOnCourt);
                    }
                }

                UpdateTeamCompositeRatings(Teams, PlayersOnCourt);

                outcome = SimPossession(Teams, PlayersOnCourt);

                // Troca Offense e Defense para que o receba outra posse quando eles forem trocados novamente no início do loop.
                if (outcome == "Orb")
                {
                    Offense = (Offense == 1) ? 0 : 1;
                    Defense = (Offense == 1) ? 0 : 1;
                }

                UpdatePlayingTime(Teams, PlayersOnCourt);

                //Injuries();

                i += 1;
            }
        }

        public bool UpdatePlayersOnCourt(Team[] teams, int[][] PlayersOnCourt)
        {
            bool substitutions = false;

            for (int t = 0; t < 2; t++)
            {
                decimal[] ovrs = new decimal[teams[t].Players.Count];

                for (int p = 0; p < teams[t].Players.Count; p++)
                {

                    var player = teams[t].Players[p];
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
                        decimal negativeInfinity = decimal.MinValue;
                        ovrs[p] = negativeInfinity;
                    }
                    else
                    {
                        ovrs[p] = teams[t].Players[p].Ratings.LastOrDefault().Ovr * Fatigue(teams[t].Players[p].Stats.Find(s => s.GameId == Id).Energy) *
                              teams[t].Players[p].PtModifier * RandomUtils.RandomUniform(0.9m, 1.1m);
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
                                if (Fatigue(teams[t].Players[p].Stats.Find(s => s.GameId == Id).Energy) > 0.7m)
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
                Dictionary<string, decimal> skillsCount = new Dictionary<string, decimal>
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

                    skillsCount["3"] += RandomUtils.Sigmoid(Teams[t].Players[p].CompositeRating.Ratings["ShootingThreePointer"], 15, 0.7m);
                    skillsCount["B"] += RandomUtils.Sigmoid(Teams[t].Players[p].CompositeRating.Ratings["Dribbling"], 15, 0.7m);
                    skillsCount["A"] += RandomUtils.Sigmoid(Teams[t].Players[p].CompositeRating.Ratings["Athleticism"], 15, 0.7m);
                    skillsCount["Di"] += RandomUtils.Sigmoid(Teams[t].Players[p].CompositeRating.Ratings["DefenseInterior"], 15, 0.7m);
                    skillsCount["Dp"] += RandomUtils.Sigmoid(Teams[t].Players[p].CompositeRating.Ratings["DefensePerimeter"], 15, 0.7m);
                    skillsCount["Po"] += RandomUtils.Sigmoid(Teams[t].Players[p].CompositeRating.Ratings["ShootingLowPost"], 15, 0.7m);
                    skillsCount["Ps"] += RandomUtils.Sigmoid(Teams[t].Players[p].CompositeRating.Ratings["Passing"], 15, 0.7m);
                    skillsCount["R"] += RandomUtils.Sigmoid(Teams[t].Players[p].CompositeRating.Ratings["Rebounding"], 15, 0.7m);
                }

                // Sinergia ofensiva de base
                Teams[t].Synergy.Off = 0;
                Teams[t].Synergy.Off += 5 * RandomUtils.Sigmoid(skillsCount["3"], 3, 2);
                Teams[t].Synergy.Off += 3 * RandomUtils.Sigmoid(skillsCount["B"], 15, 0.75m) + RandomUtils.Sigmoid(skillsCount["B"], 5, 1.75m);
                Teams[t].Synergy.Off += 3 * RandomUtils.Sigmoid(skillsCount["Ps"], 15, 0.75m) + RandomUtils.Sigmoid(skillsCount["Ps"], 5, 1.75m)
                    + RandomUtils.Sigmoid(skillsCount["Ps"], 5, 2.75m);
                Teams[t].Synergy.Off += RandomUtils.Sigmoid(skillsCount["Po"], 15, 0.75m);
                Teams[t].Synergy.Off += RandomUtils.Sigmoid(skillsCount["A"], 15, 1.75m) + RandomUtils.Sigmoid(skillsCount["A"], 5, 2.75m);
                Teams[t].Synergy.Off /= 17;

                // Punir as equipes por não terem múltiplas habilidades de perímetro
                decimal perimFactor = RandomUtils.Clamp((decimal)Math.Sqrt((double)(1 + skillsCount["B"] + skillsCount["Ps"] + skillsCount["3"])) - 1M, 0M, 2M) / 2M;
                Teams[t].Synergy.Off *= 0.5m + 0.5m * perimFactor;

                // Sinergia defensiva
                Teams[t].Synergy.Def = 0;
                Teams[t].Synergy.Def += RandomUtils.Sigmoid(skillsCount["Dp"], 15, 0.75m);
                Teams[t].Synergy.Def += 2 * RandomUtils.Sigmoid(skillsCount["Di"], 15, 0.75m);
                Teams[t].Synergy.Def += RandomUtils.Sigmoid(skillsCount["A"], 5, 2) + RandomUtils.Sigmoid(skillsCount["A"], 5, 3.25m);
                Teams[t].Synergy.Def /= 6;

                // Recuperando a sinergia
                Teams[t].Synergy.Reb = 0;
                Teams[t].Synergy.Reb += RandomUtils.Sigmoid(skillsCount["R"], 15, 0.75m) + RandomUtils.Sigmoid(skillsCount["R"], 5, 1.75m);
                Teams[t].Synergy.Reb /= 4;
            }

        }

        private void UpdateSynergy2(Team[] Teams, int[][] PlayersOnCourt)
        {
            for (int t = 0; t < 2; t++)
            {

                if (Teams[t].Synergy == null)
                {
                    Teams[t].Synergy = new TeamSynergy();
                }

                List<string> allSkills = new List<string>();
                for (int i = 0; i < 5; i++)
                {
                    int p = PlayersOnCourt[t][i];
                    allSkills.AddRange(Teams[t].Players[p].Ratings.LastOrDefault().Skills);
                }
                Dictionary<string, int> skillsCount = allSkills.GroupBy(s => s).ToDictionary(g => g.Key, g => g.Count());

                // Sinergia ofensiva
                Teams[t].Synergy.Off = 0;
                if (skillsCount.ContainsKey("3") && skillsCount["3"] >= 2) { Teams[t].Synergy.Off += 3; }
                if (skillsCount.ContainsKey("3") && skillsCount["3"] >= 3) { Teams[t].Synergy.Off += 1; }
                if (skillsCount.ContainsKey("3") && skillsCount["3"] >= 4) { Teams[t].Synergy.Off += 1; }
                if (skillsCount.ContainsKey("B") && skillsCount["B"] >= 1) { Teams[t].Synergy.Off += 3; }
                if (skillsCount.ContainsKey("B") && skillsCount["B"] >= 2) { Teams[t].Synergy.Off += 1; }
                if (skillsCount.ContainsKey("Ps") && skillsCount["Ps"] >= 1) { Teams[t].Synergy.Off += 3; }
                if (skillsCount.ContainsKey("Ps") && skillsCount["Ps"] >= 2) { Teams[t].Synergy.Off += 1; }
                if (skillsCount.ContainsKey("Ps") && skillsCount["Ps"] >= 3) { Teams[t].Synergy.Off += 1; }
                if (skillsCount.ContainsKey("Po") && skillsCount["Po"] >= 1) { Teams[t].Synergy.Off += 1; }
                if (skillsCount.ContainsKey("A") && skillsCount["A"] >= 3) { Teams[t].Synergy.Off += 1; }
                if (skillsCount.ContainsKey("A") && skillsCount["A"] >= 4) { Teams[t].Synergy.Off += 1; }
                Teams[t].Synergy.Off /= 17;

                // Sinergia defensiva
                Teams[t].Synergy.Def = 0;
                if (skillsCount.ContainsKey("Dp") && skillsCount["Dp"] >= 1) { Teams[t].Synergy.Def += 1; }
                if (skillsCount.ContainsKey("Di") && skillsCount["Di"] >= 1) { Teams[t].Synergy.Def += 3; }
                if (skillsCount.ContainsKey("A") && skillsCount["A"] >= 3) { Teams[t].Synergy.Def += 1; }
                if (skillsCount.ContainsKey("A") && skillsCount["A"] >= 4) { Teams[t].Synergy.Def += 1; }
                Teams[t].Synergy.Def /= 6;

                // Sinergia Rebotes
                Teams[t].Synergy.Reb = 0;
                if (skillsCount.ContainsKey("R") && skillsCount["R"] >= 1) { Teams[t].Synergy.Reb += 3; }
                if (skillsCount.ContainsKey("R") && skillsCount["R"] >= 2) { Teams[t].Synergy.Reb += 1; }
                Teams[t].Synergy.Reb /= 4;
            }
        }



        public void UpdateTeamCompositeRatings(Team[] teams, int[][] playersOnCourt)
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
                    decimal ratingValue = 0;
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

                    teams[i].CompositeRating.Ratings[rats] /= 5;

                }

                teams[i].CompositeRating.Ratings["GameDribbling"] += SynergyFactor * teams[i].Synergy.Off;
                teams[i].CompositeRating.Ratings["GamePassing"] += SynergyFactor * teams[i].Synergy.Off;
                teams[i].CompositeRating.Ratings["GameRebounding"] += SynergyFactor * teams[i].Synergy.Reb;
                teams[i].CompositeRating.Ratings["GameDefense"] += SynergyFactor * teams[i].Synergy.Def;
                teams[i].CompositeRating.Ratings["GameDefensePerimeter"] += SynergyFactor * teams[i].Synergy.Def;
                teams[i].CompositeRating.Ratings["GameBlocking"] += SynergyFactor * teams[i].Synergy.Def;

            }
        }

        public void UpdatePlayingTime(Team[] Teams, int[][] PlayersOnCourt)
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
                        RecordStat(t, p, "Energy", Teams, 1, (-Dt * 0.06m * (1 - Teams[t].Players[p].CompositeRating.Ratings["Endurance"])));
                        if (Teams[t].Players[p].Stats.Find(s => s.GameId == Id)?.Energy < 0)
                        {
                            Teams[t].Players[p].Stats.Find(s => s.GameId == Id).Energy = 0;
                        }
                    }
                    else
                    {
                        RecordStat(t, p, "BenchTime", Teams, 1, Dt);
                        RecordStat(t, p, "Energy", Teams, 1, (Dt * 0.1m));
                        if (Teams[t].Players[p].Stats.Find(s => s.GameId == Id)?.Energy > 1)
                        {
                            Teams[t].Players[p].Stats.Find(s => s.GameId == Id).Energy = 1;
                        }
                    }
                }
            }
        }

        private string SimPossession(Team[] Teams, int[][] PlayersOnCourt)
        {
            decimal[] ratios;
            int shooter;

            // Turnover?
            if (ProbTov(Teams) > RandomUtils.GetRandomDecimal(0, 100, 2))
            {
                return DoTov(Teams, PlayersOnCourt); // TOV
            }

            // Chutar se não houver turnover
            ratios = RatingArray(Teams, "Usage", Offense, PlayersOnCourt);
            shooter = PickPlayer(ratios);

            return DoShot(shooter, Teams, PlayersOnCourt); // Fg, Orb ou Drb
        }

        private decimal ProbTov(Team[] Teams)
        {

            decimal defenseRating = Teams[Defense].CompositeRating.Ratings["GameDefense"];
            decimal dribblingRating = Teams[Offense].CompositeRating.Ratings["GameDribbling"];
            decimal passingRating = Teams[Offense].CompositeRating.Ratings["GamePassing"];

            return 0.13m * (1 + defenseRating / (1 + 0.5m * (dribblingRating + passingRating)));
        }

        private string DoTov(Team[] Teams, int[][] PlayersOnCourt)
        {
            int p;
            decimal[] ratios;

            ratios = RatingArray(Teams, "Turnovers", Offense, PlayersOnCourt, 0.5m);
            p = PlayersOnCourt[Offense][PickPlayer(ratios)];
            RecordStat(Offense, p, "Tov", Teams);
            if (ProbStl(Teams) > RandomUtils.GetRandomDecimal(0, 100, 2))
            {
                return DoStl(Teams, PlayersOnCourt);
            }

            return "Tov";
        }

        private decimal ProbStl(Team[] Teams)
        {

            decimal defensePerimeterRating = Teams[Defense].CompositeRating.Ratings["GameDefensePerimeter"];
            decimal dribblingRating = Teams[Offense].CompositeRating.Ratings["GameDribbling"];
            decimal passingRating = Teams[Offense].CompositeRating.Ratings["GamePassing"];

            return 0.55m * defensePerimeterRating / (0.5m * (dribblingRating + passingRating));
        }

        private string DoStl(Team[] Teams, int[][] PlayersOnCourt)
        {
            decimal[] ratios = RatingArray(Teams, "Stealing", Defense, PlayersOnCourt);
            int p = PlayersOnCourt[Defense][PickPlayer(ratios)];
            RecordStat(Defense, p, "Stl", Teams);

            return "Stl";
        }

        private string DoShot(int shooter, Team[] Teams, int[][] PlayersOnCourt)
        {
            int p = PlayersOnCourt[Offense][shooter];
            decimal currentFatigue = this.Fatigue(Teams[Offense].Players[p].Stats.Find(s => s.GameId == Id).Energy);
            int passer = -1;

            if (ProbAst(Teams) > RandomUtils.GetRandomDecimal(0, 100, 2))
            {
                decimal[] ratios = RatingArray(Teams, "Passing", Offense, PlayersOnCourt, 0.5m);
                passer = PickPlayer(ratios, shooter);
            }

            decimal shootingThreePointerScaled = Teams[Offense].Players[p].CompositeRating.Ratings["ShootingThreePointer"];

            if (shootingThreePointerScaled > 0.55m)
            {
                shootingThreePointerScaled = 0.55m + (shootingThreePointerScaled - 0.55m) * (0.3m / 0.45m);
            }

            decimal shootingThreePointerScaled2 = shootingThreePointerScaled;
            if (shootingThreePointerScaled2 < 0.35m)
            {
                shootingThreePointerScaled2 = 0 + shootingThreePointerScaled2 * (0.1m / 0.35m);
            }
            else if (shootingThreePointerScaled2 < 0.45m)
            {
                shootingThreePointerScaled2 = 0.1m + (shootingThreePointerScaled2 - 0.35m) * (0.35m / 0.1m);
            }

            decimal probAndOne;
            decimal probMake;
            decimal probMissAndFoul;
            string type;

            if (RandomUtils.GetRandomDecimal(0, 100, 2) < 0.67m * shootingThreePointerScaled2 * 1)
            {
                // Three pointer
                type = "ThreePointer";
                probMissAndFoul = 0.02m;
                probMake = shootingThreePointerScaled * 0.3m + 0.36m;
                probAndOne = 0.01m;
                probMake *= 1;

            }
            else
            {
                decimal r1 = 0.8m * RandomUtils.GetRandomDecimal(0, 100, 2) * Teams[Offense].Players[p].CompositeRating.Ratings["ShootingMidRange"];
                decimal r2 = RandomUtils.GetRandomDecimal(0, 100, 2) * (Teams[Offense].Players[p].CompositeRating.Ratings["ShootingAtRim"] +
                                                        SynergyFactor * (Teams[Offense].Synergy.Off - Teams[Defense].Synergy.Def));
                decimal r3 = RandomUtils.GetRandomDecimal(0, 100, 2) * (Teams[Offense].Players[p].CompositeRating.Ratings["ShootingLowPost"] +
                                                        SynergyFactor * (Teams[Offense].Synergy.Off - Teams[Defense].Synergy.Def));

                if (r1 > r2 && r1 > r3)
                {
                    type = "MidRange";
                    probMissAndFoul = 0.07m;
                    probMake = Teams[Offense].Players[p].CompositeRating.Ratings["ShootingMidRange"] * 0.32m + 0.42m;
                    probAndOne = 0.05m;
                }
                else if (r2 > r3)
                {
                    type = "AtRim";
                    probMissAndFoul = 0.37m;
                    probMake = Teams[Offense].Players[p].CompositeRating.Ratings["ShootingAtRim"] * 0.41m + 0.54m;
                    probAndOne = 0.25m;
                }
                else
                {
                    type = "LowPost";
                    probMissAndFoul = 0.33m;
                    probMake = Teams[Offense].Players[p].CompositeRating.Ratings["ShootingLowPost"] * 0.32m + 0.34m;
                    probAndOne = 0.15m;
                }

                probMake *= 1;

            }

            decimal foulFactor = 0.65m *
                    (decimal)Math.Pow(0.5, 2) *
                    1M;

            probMissAndFoul *= foulFactor;
            probAndOne *= foulFactor;
            probMake = (probMake -
                        0.25m * Teams[Defense].CompositeRating.Ratings["GameDefense"] +
                        SynergyFactor *
                        (Teams[Offense].Synergy.Off - Teams[Defense].Synergy.Def)) *
                       currentFatigue;

            if (T == 0 && Dt < 6.0m / 60.0m)
            {
                probMake *= (decimal)Math.Sqrt((double)Dt / (8.0 / 60.0));
            }

            if (passer > 0)
            {
                probMake += 0.025m;
            }

            if (ProbBlk(Teams) > RandomUtils.GetRandomDecimal(0, 100, 2))
            {
                return DoBlk(shooter, type, Teams, PlayersOnCourt);
            }

            if (probMake > RandomUtils.GetRandomDecimal(0, 100, 2))
            {
                // And 1
                if (probAndOne > RandomUtils.GetRandomDecimal(0, 100, 2))
                {
                    return DoFg(shooter, passer, type, Teams, PlayersOnCourt);
                }

                return DoFg(shooter, passer, type, Teams, PlayersOnCourt);
            }

            if (probMissAndFoul > RandomUtils.GetRandomDecimal(0, 100, 2))
            {
                bool threePointer = type == "ThreePointer";

                DoPf(Defense, Teams, PlayersOnCourt);

                if (threePointer)
                {
                    return DoFt(shooter, 3, Teams, PlayersOnCourt);
                }

                return DoFt(shooter, 2, Teams, PlayersOnCourt);
            }

            p = PlayersOnCourt[Offense][shooter];
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

        private decimal ProbBlk(Team[] Teams)
        {

            decimal defenseBlocking = Teams[Defense].CompositeRating.Ratings["GameBlocking"];

            return 0.1m * defenseBlocking;
        }

        private string DoBlk(int shooter, string type, Team[] Teams, int[][] PlayersOnCourt)
        {
            decimal[] ratios = RatingArray(Teams, "Blocking", Defense, PlayersOnCourt, 4);
            int p = PlayersOnCourt[Defense][PickPlayer(ratios)];
            RecordStat(Defense, p, "Blk", Teams);

            int p2 = PlayersOnCourt[Offense][shooter];
            RecordStat(Offense, p2, "Fga", Teams);
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

        private string DoFg(int shooter, int passer, string type, Team[] Teams, int[][] PlayersOnCourt)
        {
            int p;

            if (passer >= 0)
            {
                p = PlayersOnCourt[Offense][passer];
                RecordStat(Offense, p, "Ast", Teams);
            }


            p = PlayersOnCourt[Offense][shooter];
            RecordStat(Offense, p, "Fga", Teams);
            RecordStat(Offense, p, "Fg", Teams);
            RecordStat(Offense, p, "Pts", Teams, 2);

            switch (type)
            {
                case "AtRim":
                    RecordStat(Offense, p, "FgaAtRim", Teams);
                    RecordStat(Offense, p, "FgAtRim", Teams);
                    break;
                case "LowPost":
                    RecordStat(Offense, p, "FgaLowPost", Teams);
                    RecordStat(Offense, p, "FgLowPost", Teams);
                    break;
                case "MidRange":
                    RecordStat(Offense, p, "FgaMidRange", Teams);
                    RecordStat(Offense, p, "FgMidRange", Teams);
                    break;
                case "ThreePointer":
                    RecordStat(Offense, p, "Pts", Teams);
                    RecordStat(Offense, p, "Tpa", Teams);
                    RecordStat(Offense, p, "Tp", Teams);
                    break;
            }

            return "Fg";
        }

        private decimal ProbAst(Team[] Teams)
        {
            decimal passing = Teams[Offense].CompositeRating.Ratings["GamePassing"];
            decimal defense = Teams[Defense].CompositeRating.Ratings["GameDefense"];
            return 0.6m * (2m + passing / (2m + defense));
        }

        private string DoFt(int shooter, int amount, Team[] Teams, int[][] PlayersOnCourt)
        {
            DoPf(Defense, Teams, PlayersOnCourt);
            int p = PlayersOnCourt[Offense][shooter];

            var player = Teams[Offense].Players[p];

            string outcome = string.Empty;
            for (int i = 0; i < amount; i++)
            {
                RecordStat(Offense, p, "Fta", Teams);
                if (RandomUtils.GetRandomDecimal(0, 100, 2) < player.CompositeRating.Ratings["ShootingFT"] * 0.3m + 0.6m) // Entre 60% e 90%
                {
                    RecordStat(Offense, p, "Ft", Teams);
                    RecordStat(Offense, p, "Pts", Teams);
                    outcome = "Fg";
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

            decimal[] ratios = RatingArray(Teams, "Fouling", t, PlayersOnCourt);
            int p = PlayersOnCourt[t][PickPlayer(ratios)];
            RecordStat(Defense, p, "Pf", Teams);

            var player = Teams[Defense].Players[p];

            if (player.Stats.Find(s => s.GameId == Id).Pf >= 6)
            {
                UpdatePlayersOnCourt(Teams, PlayersOnCourt);
                UpdateSynergy(Teams, PlayersOnCourt);
            }
        }

        private string DoReb(Team[] Teams, int[][] PlayersOnCourt)
        {
            if (new Random().NextDouble() < 0.15)
            {
                return null;
            }

            decimal defenseRebounding = Teams[Defense].CompositeRating.Ratings["GameRebounding"];
            decimal offenseRebounding = Teams[Offense].CompositeRating.Ratings["GameRebounding"];

            if (0.75m * (2 + defenseRebounding) / (2 + offenseRebounding) > RandomUtils.GetRandomDecimal(0, 100, 2))
            {
                decimal[] ratios = RatingArray(Teams, "Rebounding", Defense, PlayersOnCourt);
                int p = PlayersOnCourt[Defense][PickPlayer(ratios)];
                RecordStat(Defense, p, "Drb", Teams);
                RecordStat(Defense, p, "Trb", Teams);

                return "Drb";
            }

            decimal[] oRatios = RatingArray(Teams, "Rebounding", Offense, PlayersOnCourt);
            int oP = PlayersOnCourt[Offense][PickPlayer(oRatios)];
            RecordStat(Offense, oP, "Orb", Teams);
            RecordStat(Offense, oP, "Trb", Teams);

            return "Orb";
        }

        private decimal[] RatingArray(Team[] Teams, string rating, int t, int[][] PlayersOnCourt, decimal power = 1)
        {
            decimal[] array = new decimal[5];

            for (int i = 0; i < 5; i++)
            {
                int p = PlayersOnCourt[t][i];
                array[i] = (decimal)Math.Pow((double)(Teams[t].Players[p].CompositeRating.Ratings[rating] *
                     Fatigue(Teams[t].Players[p].Stats.Find(s => s.GameId == Id).Energy)), (double)power);

            }

            return array;
        }


        private int PickPlayer(decimal[] ratios, int? exempt = null)
        {
            if (exempt.HasValue)
            {
                ratios[exempt.Value] = 0;
            }

            decimal sum = ratios.Sum();

            if (sum == 0)
            {
                var candidates = Enumerable.Range(0, ratios.Length).Where(i => i != exempt).ToList();
                Random random = new Random();
                return candidates[random.Next(candidates.Count)];
            }

            decimal rand = RandomUtils.GetRandomDecimal(0, 100, 2) * sum;

            decimal runningSum = 0;

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

        private void RecordStat(int t, int p, string s, Team[] teams, int amount = 1, decimal amntDouble = 1.0m)
        {
            amount = amount != 0 ? amount : 1;
            RecordHelper.RecordStatHelperPlayer(GameDate, t, p, s, Id, teams, Type, Season.Year, amount, amntDouble);
            if (s != "Gs" && s != "CourtTime" && s != "BenchTime" && s != "Energy")
            {
                RecordHelper.RecordStatHelperTeam(t, p, s, Id, teams, Season.Year, amount);

            }
        }

        private decimal Fatigue(decimal energy)
        {
            energy += 0.05m;
            if (energy > 1)
            {
                energy = 1;
            }

            return energy;
        }

    }

}
