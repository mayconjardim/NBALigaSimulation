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
        double SynergyFactor = 0.05; // Qual a importância da sinergia?
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

        private void GameSim()
        {

            Team[] Teams = { HomeTeam, AwayTeam };
            CompositeHelper.UpdatePace(Teams);

            NumPossessions = (int)Math.Round(Teams[0].CompositeRating.Ratings["GamePace"] + Teams[1].CompositeRating.Ratings["GamePace"] / 2 * RandomUtils.RandomUniform(0.9, 1.1));

            int[][] PlayersOnCourt = new int[][] { new int[] { 0, 1, 2, 3, 4 }, new int[] { 0, 1, 2, 3, 4 } };

            UpdatePlayersOnCourt(Teams, PlayersOnCourt);
            UpdateSynergy(Teams, PlayersOnCourt);

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
                SimPossessions(Teams, PlayersOnCourt);
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

        public bool UpdatePlayersOnCourt(Team[] Teams, int[][] PlayersOnCourt)
        {
            bool substitutions = false;

            for (int t = 0; t < 2; t++)
            {
                // Ovr por fadiga
                double[] ovrs = new double[Teams[t].Players.Count];
                for (int p = 0; p < Teams[t].Players.Count; p++)
                {

                    var player = Teams[t].Players.Find(player => player.RosterOrder == p);
                    if (player != null)
                    {
                        if (player.Stats == null)
                        {
                            player.Stats = new List<PlayerGameStats>();
                        }

                        var lastStats = player.Stats.Find(s => s.GameId == Id);

                        if (lastStats == null || lastStats.GameId != Id)
                        {
                            player.Stats.Add(new PlayerGameStats { GameId = Id, TeamId = Teams[t].Id, Name = player.Name, Season = Season.Year });
                        }
                    }

                    // Jogadores lesionados ou com falta não podem jogar
                    if (Teams[t].Players[p].Stats.Find(s => s.GameId == Id).Pf >= 6)
                    {
                        ovrs[p] = double.NegativeInfinity;
                    }
                    else
                    {
                        ovrs[p] = Teams[t].Players[p].Ratings.LastOrDefault().Ovr * Fatigue(Teams[t].Players[p].Stats.Find(s => s.GameId == Id).Energy) *
                              Teams[t].Players[p].PtModifier * RandomUtils.RandomUniform(0.9, 1.1);
                    }
                }

                // Percorre os jogadores na quadra (na ordem inversa da posição atual da lista)
                int i = 0;
                for (int pp = 0; pp < PlayersOnCourt[t].Length; pp++)
                {
                    int p = PlayersOnCourt[t][pp];
                    PlayersOnCourt[t][i] = p;

                    // Faz um loop pelos jogadores de banco (na ordem da posição atual da lista) para ver se algum deve ser substituído)
                    for (int b = 0; b < Teams[t].Players.Count; b++)
                    {
                        if (PlayersOnCourt[t].Contains(b) == false && ((Teams[t].Players[p].Stats.Find(s => s.GameId == Id).CourtTime > 3 && Teams[t].Players[b].Stats.Find(s => s.GameId == Id).BenchTime > 3
                            && ovrs[b] > ovrs[p]) || Teams[t].Players[p].Stats.Find(s => s.GameId == Id).Pf >= 6))
                        {
                            substitutions = true;

                            // Jogador substituto
                            PlayersOnCourt[t][i] = b;
                            p = b;
                            Teams[t].Players[b].Stats.Find(s => s.GameId == Id).CourtTime = RandomUtils.RandomUniform(-2, 2);
                            Teams[t].Players[b].Stats.Find(s => s.GameId == Id).BenchTime = RandomUtils.RandomUniform(-2, 2);
                            Teams[t].Players[p].Stats.Find(s => s.GameId == Id).CourtTime = RandomUtils.RandomUniform(-2, 2);
                            Teams[t].Players[p].Stats.Find(s => s.GameId == Id).BenchTime = RandomUtils.RandomUniform(-2, 2);
                        }
                    }
                    i += 1;
                }
            }

            // Grave iniciadores se isso ainda não tiver sido feito. Isso deve ser executado na primeira vez que essa função for chamada e nunca mais.
            if (!StartersRecorded)
            {
                for (int t = 0; t < 2; t++)
                {
                    for (int p = 0; p < Teams[t].Players.Count; p++)
                    {
                        if (PlayersOnCourt[t].Contains(p))
                        {
                            RecordStat(t, p, "Gs");
                        }
                    }
                }
                StartersRecorded = true;
            }

            return substitutions;
        }

        public void UpdateSynergy(Team[] Teams, int[][] PlayersOnCourt)
        {
            for (int t = 0; t < 2; t++)
            {

                if (Teams[t].Synergy == null)
                {
                    Teams[t].Synergy = new TeamSynergy();
                }

                // Faz uma lista com todas as habilidades dos jogadores ativos em uma equipe (incluindo duplicatas)
                List<string> allSkills = new List<string>();
                for (int i = 0; i < 5; i++)
                {
                    int p = PlayersOnCourt[t][i];
                    allSkills.AddRange(Teams[t].Players[p].Ratings.LastOrDefault().Skills);
                }
                Dictionary<string, int> skillsCount = new Dictionary<string, int>();
                foreach (string skill in allSkills)
                {
                    if (skillsCount.ContainsKey(skill))
                    {
                        skillsCount[skill]++;
                    }
                    else
                    {
                        skillsCount[skill] = 1;
                    }
                }

                // Sinergia ofensiva
                Teams[t].Synergy.Off = 0;
                Teams[t].Synergy.Off += (skillsCount.ContainsKey("3") && skillsCount["3"] >= 2) ? 3 : 0;
                Teams[t].Synergy.Off += (skillsCount.ContainsKey("3") && skillsCount["3"] >= 3) ? 1 : 0;
                Teams[t].Synergy.Off += (skillsCount.ContainsKey("3") && skillsCount["3"] >= 4) ? 1 : 0;
                Teams[t].Synergy.Off += (skillsCount.ContainsKey("B") && skillsCount["B"] >= 1) ? 3 : 0;
                Teams[t].Synergy.Off += (skillsCount.ContainsKey("B") && skillsCount["B"] >= 2) ? 1 : 0;
                Teams[t].Synergy.Off += (skillsCount.ContainsKey("Ps") && skillsCount["Ps"] >= 1) ? 3 : 0;
                Teams[t].Synergy.Off += (skillsCount.ContainsKey("Ps") && skillsCount["Ps"] >= 2) ? 1 : 0;
                Teams[t].Synergy.Off += (skillsCount.ContainsKey("Ps") && skillsCount["Ps"] >= 3) ? 1 : 0;
                Teams[t].Synergy.Off += (skillsCount.ContainsKey("Po") && skillsCount["Po"] >= 1) ? 1 : 0;
                Teams[t].Synergy.Off += (skillsCount.ContainsKey("A") && skillsCount["A"] >= 3) ? 1 : 0;
                Teams[t].Synergy.Off += (skillsCount.ContainsKey("A") && skillsCount["A"] >= 4) ? 1 : 0;
                Teams[t].Synergy.Off /= 17;

                // Sinergia defensiva
                Teams[t].Synergy.Def = 0;
                Teams[t].Synergy.Def += (skillsCount.ContainsKey("Dp") && skillsCount["Dp"] >= 1) ? 1 : 0;
                Teams[t].Synergy.Def += (skillsCount.ContainsKey("Di") && skillsCount["Di"] >= 1) ? 3 : 0;
                Teams[t].Synergy.Def += (skillsCount.ContainsKey("A") && skillsCount["A"] >= 3) ? 1 : 0;
                Teams[t].Synergy.Def += (skillsCount.ContainsKey("A") && skillsCount["A"] >= 4) ? 1 : 0;
                Teams[t].Synergy.Def /= 6;

                // Sinergia rebotes
                Teams[t].Synergy.Reb = 0;
                Teams[t].Synergy.Reb += (skillsCount.ContainsKey("R") && skillsCount["R"] >= 1) ? 3 : 0;
                Teams[t].Synergy.Reb += (skillsCount.ContainsKey("R") && skillsCount["R"] >= 2) ? 1 : 0;
                Teams[t].Synergy.Reb /= 4;
            }
        }

        public void UpdateTeamCompositeRatings(Team[] Teams, int[][] PlayersOnCourt)
        {
            // Atualize apenas aqueles que são realmente usados
            string[] toUpdate = { "GameDribbling", "GamePassing", "GameRebounding", "GameDefense", "GameDefensePerimeter", "GameBlocking", "GamePace" };
            string ratings = string.Empty;


            for (int t = 0; t < 2; t++)
            {

                if (Teams[t].CompositeRating == null)
                {
                    Teams[t].CompositeRating = new TeamCompositeRating();
                }

                foreach (string rating in toUpdate)
                {
                    Teams[t].CompositeRating.Ratings[rating] = 0;

                    for (int i = 0; i < 5; i++)
                    {
                        int playerRosterOrder = PlayersOnCourt[t][i];
                        var playerRatings = Teams[i].Players.Find(player => player.RosterOrder == playerRosterOrder).Ratings.LastOrDefault();
                        double ratingValue = 0;

                        if (rating == "GameDribbling")
                        {
                            ratingValue = playerRatings.GameDribbling;
                        }
                        else if (rating == "GamePassing")
                        {
                            ratingValue = playerRatings.GamePassing;
                        }
                        else if (rating == "GameRebounding")
                        {
                            ratingValue = playerRatings.GameRebounding;
                        }
                        else if (rating == "GameDefense")
                        {
                            ratingValue = playerRatings.GameDefense;
                        }
                        else if (rating == "GameDefensePerimeter")
                        {
                            ratingValue = playerRatings.GameDefensePerimeter;
                        }
                        else if (rating == "GameBlocking")
                        {
                            ratingValue = playerRatings.GameBlocking;
                        }

                        Teams[i].CompositeRating.Ratings[rating] += ratingValue * Fatigue(Teams[i].Players.Find(player => player.RosterOrder == playerRosterOrder).Stats.Find(s => s.GameId == Id).Energy);
                        ratings = rating;

                    }

                    Teams[t].CompositeRating.Ratings[ratings] = Teams[t].CompositeRating.Ratings[ratings] / 5;
                }

                Teams[t].CompositeRating.Ratings["GameDribbling"] += SynergyFactor * Teams[t].Synergy.Off;
                Teams[t].CompositeRating.Ratings["GamePassing"] += SynergyFactor * Teams[t].Synergy.Off;
                Teams[t].CompositeRating.Ratings["GameRebounding"] += SynergyFactor * Teams[t].Synergy.Reb;
                Teams[t].CompositeRating.Ratings["GameDefense"] += SynergyFactor * Teams[t].Synergy.Def;
                Teams[t].CompositeRating.Ratings["GameDefensePerimeter"] += SynergyFactor * Teams[t].Synergy.Def;
                Teams[t].CompositeRating.Ratings["GameBlocking"] += SynergyFactor * Teams[t].Synergy.Def;
            }
        }

        public void UpdatePlayingTime(Team[] Teams, int[][] PlayersOnCourt)
        {
            // Tempo decorrido
            Dt = (Overtimes > 0 ? 5 : 48) / (2 * NumPossessions);

            for (int t = 0; t < 2; t++)
            {
                // Atualiza minutos (Ovr, quadra e banco)
                for (int p = 0; p < Teams[t].Players.Count; p++)
                {
                    if (PlayersOnCourt[t].Contains(p))
                    {
                        RecordStat(t, p, "Min", Teams, 1, Dt);
                        RecordStat(t, p, "CourtTime", Teams, 1, Dt);
                        RecordStat(t, p, "Energy", Teams, 1, (-Dt * 0.04 * (1 - Teams[t].Players[p].Ratings.Last().GameEndurance)));
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

        public string SimPossession(Team[] Teams, int[][] PlayersOnCourt)
        {
            double[] ratios;
            int shooter;

            // Turnover?
            if (ProbTov(Teams) > new Random().NextDouble())
            {
                return DoTov(); // TOV
            }

            // Chutar se não houver turnover
            ratios = RatingArray(Teams, "GameUsage", Offense, PlayersOnCourt);
            shooter = ArrayHelper.PickPlayer(ratios);

            return DoShot(shooter); // Fg, Orb ou Drb
        }

        public double ProbTov(Team[] Teams)
        {

            double defenseRating = Teams[Defense].CompositeRating.Ratings["GameDefense"];
            double dribblingRating = Teams[Offense].CompositeRating.Ratings["GameDribbling"];
            double passingRating = Teams[Offense].CompositeRating.Ratings["GamePassing"];

            return 0.15 * (1 + defenseRating / (1 + 0.5 * (dribblingRating + passingRating)));
        }

        public string DoTov(Team[] Teams, int[][] PlayersOnCourt)
        {
            int p;
            double[] ratios;

            ratios = RatingArray(Teams, "GameTurnovers", Offense, PlayersOnCourt, 0.5);
            p = PlayersOnCourt[Offense][PickPlayer(ratios)];
            RecordStat(Offense, p, "Tov", Teams);
            if (ProbStl(Teams) > new Random().NextDouble())
            {
                return DoStl();
            }

            return "Tov";
        }

        public double ProbStl(Team[] Teams)
        {

            double defensePerimeterRating = Teams[Defense].CompositeRating.Ratings["GameDefensePerimeter"];
            double dribblingRating = Teams[Offense].CompositeRating.Ratings["GameDribbling"];
            double passingRating = Teams[Offense].CompositeRating.Ratings["GamePassing"];

            return 0.55 * defensePerimeterRating / (0.5 * (dribblingRating + passingRating));
        }

        public string DoStl(Team[] Teams, int[][] PlayersOnCourt)
        {
            double[] ratios = RatingArray(Teams, "GameStealing", Defense, PlayersOnCourt);
            int p = PlayersOnCourt[Defense][PickPlayer(ratios)];
            RecordStat(Defense, p, "Stl", Teams);

            return "Stl";
        }

        public string DoShot(int shooter, Team[] Teams, int[][] PlayersOnCourt)
        {
            double fatigue, passer, probMake, probAndOne, probMissAndFoul, r1, r2, r3;
            int p, ratios;
            string type;

            p = PlayersOnCourt[Offense][shooter];

            var player = Teams[Offense].Players.Find(player => player.RosterOrder == p);

            fatigue = Fatigue(Teams[Offense].Players[p].Stats.Find(s => s.GameId == Id).Energy);

            // Esta é uma tentativa "assistencia" (ou seja, uma assistência será registrada se for feita)
            passer = -1;
            if (ProbAst() > new Random().NextDouble())
            {
                ratios = RatingArray(Teams, "GamePassing", Offense, PlayersOnCourt, 2);
                passer = PickPlayer(ratios, shooter);
            }

            // Escolha o tipo de cute e armazene a taxa de sucesso (sem defesa) em probMake e a probabilidade de acerto e falta em probAndOne
            if (player.Ratings.LastOrDefault().GameShootingThreePointer > 0.4 && new Random().NextDouble() < (0.35 * player.Ratings.LastOrDefault().GameShootingThreePointer))
            {
                // Three pointer
                type = "ThreePointer";
                probMissAndFoul = 0.02;
                probMake = player.Ratings.LastOrDefault().GameShootingThreePointer * 0.68;
                probAndOne = 0.01;
            }
            else
            {
                r1 = new Random().NextDouble() * player.Ratings.LastOrDefault().GameShootingMidRange;
                r2 = new Random().NextDouble() * (player.Ratings.LastOrDefault().GameShootingAtRim + SynergyFactor * (Teams[Offense].Synergy.Off - Teams[Defense].Synergy.Def)); // A sinergia torna os chutes fáceis mais prováveis ou menos prováveis
                r3 = new Random().NextDouble() * player.Ratings.LastOrDefault().GameShootingLowPost;
                if (r1 > r2 && r1 > r3)
                {
                    // Two point jumper
                    type = "MidRange";
                    probMissAndFoul = 0.07;
                    probMake = player.Ratings.LastOrDefault().GameShootingMidRange * 0.3 + 0.29;
                    probAndOne = 0.05;
                }
                else if (r2 > r3)
                {
                    // Dunk ou Layup
                    type = "AtRim";
                    probMissAndFoul = 0.37;
                    probMake = player.Ratings.LastOrDefault().GameShootingAtRim * 0.3 + 0.52;
                    probAndOne = 0.25;
                }
                else
                {
                    // Post up
                    type = "LowPost";
                    probMissAndFoul = 0.33;
                    probMake = player.Ratings.LastOrDefault().GameShootingLowPost * 0.3 + 0.37;
                    probAndOne = 0.15;
                }
            }

            probMake = (probMake - 0.25 * Teams[Defense].CompositeRating.Ratings["GameDefense"] + SynergyFactor * (Teams[Offense].Synergy.Off - Teams[Defense].Synergy.Def)) * fatigue;

            // chutes com assistencia são mais fáceis
            if (passer >= 0)
            {
                probMake += 0.025;
            }

            if (ProbBlk(Teams) > new Random().NextDouble())
            {
                return DoBlk(shooter, type, Teams, PlayersOnCourt);  // orb or drb
            }

            // Acerto
            if (probMake > new Random().NextDouble())
            {
                // And 1
                if (probAndOne > new Random().NextDouble())
                {
                    DoFg(shooter, passer, type, true, Teams, PlayersOnCourt);
                    return DoFt(shooter, 1);  // fg, orb, or drb
                }
                return DoFg(shooter, passer, type, false, Teams, PlayersOnCourt);   // fg
            }

            // Errou, mas sofreu falta
            if (probMissAndFoul > new Random().NextDouble())
            {
                if (type == "ThreePointer")
                {
                    return DoFt(shooter, 3, Teams, PlayersOnCourt);  // fg, orb, or drb
                }
                return DoFt(shooter, 2, Teams, PlayersOnCourt); // fg, orb, or drb
            }

            // errou
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

        public double ProbBlk(Team[] Teams)
        {

            double defenseBlocking = Teams[Defense].CompositeRating.Ratings["GameBlocking"];

            return 0.1 * defenseBlocking;
        }

        public string DoBlk(int shooter, string type, Team[] Teams, int[][] PlayersOnCourt)
        {
            double[] ratios = RatingArray(Teams, "GameBlocking", Defense, PlayersOnCourt, 4);
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

            return DoReb();
        }

        public string DoFg(int shooter, int passer, string type, Team[] Teams, int[][] PlayersOnCourt)
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

        public double ProbAst(Team[] Teams)
        {
            double passing = Teams[Offense].CompositeRating.Ratings["GamePassing"];
            double defense = Teams[Defense].CompositeRating.Ratings["GameDefense"];
            return 0.6 * (2 + passing / (2 + defense));
        }

        public string DoFt(int shooter, int amount, Team[] Teams, int[][] PlayersOnCourt)
        {
            DoPf(Defense, Teams, PlayersOnCourt);
            int p = PlayersOnCourt[Offense][shooter];

            var player = Teams[Offense].Players.Find(player => player.RosterOrder == p);

            string outcome = string.Empty;
            for (int i = 0; i < amount; i++)
            {
                RecordStat(Offense, p, "Fta", Teams);
                if (new Random().NextDouble() < player.Ratings.LastOrDefault().GameShootingFT * 0.3 + 0.6) // Entre 60% e 90%
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

        public void DoPf(int t, Team[] Teams, int[][] PlayersOnCourt)
        {

            double[] ratios = RatingArray(Teams, "GameFouling", t, PlayersOnCourt);
            int p = PlayersOnCourt[t][PickPlayer(ratios)];
            RecordStat(Defense, p, "Pf", Teams);

            var player = Teams[Defense].Players.Find(player => player.RosterOrder == p);

            if (player.Stats.Find(s => s.GameId == Id).Pf >= 6)
            {
                UpdatePlayersOnCourt(Teams, PlayersOnCourt);
                UpdateSynergy(Teams, PlayersOnCourt);
            }
        }

        public string DoReb(Team[] Teams, int[][] PlayersOnCourt)
        {
            if (0.15 > new Random().NextDouble())
            {
                return null;
            }

            double defenseRebounding = Teams[Defense].CompositeRating.Ratings["GameRebounding"];
            double offenseRebounding = Teams[Offense].CompositeRating.Ratings["GameRebounding"];


            if (0.75 * (2 + defenseRebounding) / (2 + offenseRebounding) > new Random().NextDouble())
            {
                double[] ratios = RatingArray(Teams, "GameRebounding", Defense, PlayersOnCourt);
                int p = PlayersOnCourt[Defense][PickPlayer(ratios)];
                RecordStat(Defense, p, "Drb", Teams);
                RecordStat(Defense, p, "Trb", Teams);

                return "Drb";
            }

            double[] oRatios = RatingArray(Teams, "GameRebounding", Offense, PlayersOnCourt);
            int oP = PlayersOnCourt[Offense][PickPlayer(oRatios)];
            RecordStat(Offense, oP, "Orb", Teams);
            RecordStat(Offense, oP, "Trb", Teams);

            return "Orb";
        }

        public double[] RatingArray(Team[] Teams, string rating, int t, int[][] PlayersOnCourt, double power = 1)
        {
            double[] array = new double[5];
            for (int i = 0; i < 5; i++)
            {
                int p = PlayersOnCourt[t][i];

                var player = Teams[t].Players.Find(player => player.RosterOrder == p);

                double energy = (double)(player.Stats.Find(s => s.GameId == Id)?.Energy);

                if (player != null && player.Ratings?.LastOrDefault() != null)
                {
                    var playerRatings = player.Ratings.LastOrDefault();
                    array[i] = Math.Pow(CompositeHelper.GetRatingValue(rating, playerRatings) * energy, power);
                }
            }

            return array;
        }

    }

}
