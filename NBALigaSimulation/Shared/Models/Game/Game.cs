using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

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
        double SynergyFactor = 0.05; //Qual a importância da sinergia?
        [NotMapped]
        int Offense; //Time que está atacando
        [NotMapped]
        int Defense; //Time que está defendendo


        //Simulação
        public void GameSim()
        {
            //Atribui os times para simulação
            Team[] teams = { HomeTeam, AwayTeam };

            //Gerando a quantidade posses de bola tem uma partida *(98 + 101 substituir por pace dos times) 
            Random random = new Random();
            double randomFactor = random.NextDouble() * (1.1 - 0.9) + 0.9;
            NumPossessions = (int)Math.Round((98 + 101) / 2 * randomFactor);

            // Escalações iniciais, que serão redefinidas por updatePlayersOnCourt. Isso deve ser feito por causa dos jogadores lesionados no top 5.
            int[][] playersOnCourt = new int[2][];
            playersOnCourt[0] = HomeTeam.Players.Where(ply => ply.RosterOrder < 5).Select(ply => ply.Id).ToArray();
            playersOnCourt[1] = AwayTeam.Players.Where(ply => ply.RosterOrder < 5).Select(ply => ply.Id).ToArray();

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

                //UpdateTeamCompositeRatings();

                // outcome = SimPossession();
                outcome = "zeca";

                // Troca Offense e Defense para que o receba outra posse quando eles forem trocados novamente no início do loop.
                if (outcome == "Orb")
                {
                    Offense = (Offense == 1) ? 0 : 1;
                    Defense = (Defense == 1) ? 0 : 1;
                }

                //UpdatePlayingTime();

                //Injuries();

                i += 1;

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
                    if (teams[t].Players.Find(player => player.RosterOrder == p).Stats.LastOrDefault() == null)
                    {
                        teams[t].Players.Find(player => player.RosterOrder == p).Stats.Add(new PlayerGameStats());
                        teams[t].Players.Find(player => player.RosterOrder == p).Stats.LastOrDefault().GameId = Id;

                    }

                    // Jogadores lesionados ou com falta não podem jogar //teams[t].Players[p].injured
                    if (teams[t].Players[p].Stats.LastOrDefault().Pf >= 6)
                    {
                        ovrs[p] = double.NegativeInfinity;
                    }
                    else
                    {
                        ovrs[p] = teams[t].Players[p].Ratings.LastOrDefault().Ovr * Fatigue(teams[t].Players[p].Stats.LastOrDefault().Energy) * teams[t].Players[p].PtModifier; // * RandomUniform(0.9, 1.1);
                    }
                }

                // Percorre os jogadores na quadra (na ordem inversa da posição atual da lista)
                int i = 0;
                for (int pp = 0; pp < playersOnCourt[t].Length; pp++)
                {

                    int p = playersOnCourt[t][pp];
                    playersOnCourt[t][i] = p;

                    // Loop through bench players (in order of current roster position) to see if any should be subbed in)
                    for (int b = 0; b < teams[t].Players.Count; b++)
                    {
                        if (!playersOnCourt[t].Any(p => p == teams[t].Players[b].Id) &&
                            teams[t].Players.Find(player => player.Id == p).Stats.LastOrDefault().CourtTime > 3 &&
                            teams[t].Players.Find(player => player.Id == p).Stats.LastOrDefault().BenchTime > 3 &&
                            ovrs[b] > ovrs[p] ||
                            teams[t].Players.Find(player => player.Id == p).Stats.LastOrDefault().Pf >= 6)
                        {
                            substitutions = true;

                            // Substitute player
                            playersOnCourt[t][i] = teams[t].Players[b].Id;
                            p = b;

                            teams[t].Players[b].Stats.LastOrDefault().CourtTime = RandomUniform(-2, 2);
                            teams[t].Players[b].Stats.LastOrDefault().BenchTime = RandomUniform(-2, 2);
                            teams[t].Players[p].Stats.LastOrDefault().CourtTime = RandomUniform(-2, 2);
                            teams[t].Players[p].Stats.LastOrDefault().BenchTime = RandomUniform(-2, 2);
                        }
                    }

                    i += 1;
                }

            }

            return substitutions;
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

        private double RandomUniform(double a, double b)
        {
            return new Random().NextDouble() * (b - a) + a;
        }

    }
}
