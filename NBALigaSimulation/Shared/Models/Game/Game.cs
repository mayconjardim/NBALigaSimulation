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

            UpdatePlayingTime(possessionTime);

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




    }

}