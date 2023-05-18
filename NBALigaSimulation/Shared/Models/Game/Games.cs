using NBALigaSimulation.Shared.Engine;
using NBALigaSimulation.Shared.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace NBALigaSimulation.Shared.Model
{
    public class Games
    {

        public int Id { get; set; }
        public int HomeTeamId { get; set; }
        [ForeignKey("HomeTeamId")]
        public Team? HomeTeam { get; set; }
        public int AwayTeamId { get; set; }
        [ForeignKey("AwayTeamId")]
        public Team? AwayTeam { get; set; }
        public List<TeamGameStats>? TeamGameStats { get; set; }
        public List<PlayerGameStats>? PlayerGameStats { get; set; }

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
        int T = 12; // Tempo por quarto
        [NotMapped]
        int Dt;     // Tempo decorrido por posse

        [NotMapped] // Jogadores em quadra
        int[][] PlayersOnCourt = new int[][] { new int[] { 0, 1, 2, 3, 4 }, new int[] { 0, 1, 2, 3, 4 } };

        public void GameSim()
        {

            Team[] Teams = { HomeTeam, AwayTeam };
            NumPossessions = (int)Math.Round(92.7 + 98.7 / 2 * RandomUtils.RandomUniform(0.9, 1.1));
            Dt = 48 / (2 * NumPossessions);

            // Simula o jogo até o final da partida
            SimPossessions();

            // Jogue períodos de prorrogação se necessário
            while (Teams[0].Stats.Find(s => s.GameId == Id)?.Pts == Teams[1].Stats.Find(s => s.GameId == Id)?.Pts)
            {
                if (Overtimes == 0)
                {
                    NumPossessions = (int)Math.Round(NumPossessions * 5.0 / 48); // 5 minutos de posses
                    Dt = (int)5.0 / (2 * NumPossessions);
                }

                T = 5;
                Overtimes++;
                Teams[0].Stats.Find(s => s.GameId == Id)?.PtsQtrs.Add(new PtsQtr { Points = 0 });
                Teams[1].Stats.Find(s => s.GameId == Id)?.PtsQtrs.Add(new PtsQtr { Points = 0 });
                RecordPlay("Overtime");
                SimPossessions(Teams);
            }

        }

        public void SimPossessions(Team[] Teams)
        {
            int i = 0;
            while (i < NumPossessions * 2)
            {
                // Acompanha os quartos
                if ((i * Dt > 12 && Teams[0].Stats.Find(s => s.GameId == Id)?.PtsQtrs.Count == 1) ||
                    (i * Dt > 24 && Teams[0].Stats.Find(s => s.GameId == Id)?.PtsQtrs.Count == 2) ||
                    (i * Dt > 36 && Teams[0].Stats.Find(s => s.GameId == Id)?.PtsQtrs.Count == 3))
                {
                    Teams[0].Stats.Find(s => s.GameId == Id)?.PtsQtrs.Add(new PtsQtr { Points = 0 });
                    Teams[1].Stats.Find(s => s.GameId == Id)?.PtsQtrs.Add(new PtsQtr { Points = 0 });
                    T = 12;
                    RecordPlay("Quarter");
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

                UpdateTeamCompositeRatings();

                string outcome = SimPossession();

                // Troca o e d para que o receba outra posse quando eles forem trocados novamente no início do loop.
                if (outcome == "Orb")
                {
                    Offense = (Offense == 1) ? 0 : 1;
                    Defense = (Offense == 1) ? 0 : 1;
                }

                UpdatePlayingTime();

                //Injuries();

                if (i % SubsEveryN == 0)
                {
                    bool substitutions = UpdatePlayersOnCourt();
                    if (substitutions)
                    {
                        UpdateSynergy();
                    }
                }

                i += 1;
            }
        }


    }
}
