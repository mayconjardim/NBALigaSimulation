using System;
using System.ComponentModel.DataAnnotations.Schema;


namespace NBALigaSimulation.Shared.Models
{
    [NotMapped]
    public class Games
    {

        public int Id { get; set; }
        public int HomeTeamId { get; set; }
        //[ForeignKey("HomeTeamId")]
        public Team HomeTeam { get; set; }
        public int AwayTeamId { get; set; }
        //[ForeignKey("AwayTeamId")]
        public Team AwayTeam { get; set; }


        //Atributos Globais da Simulação
        [NotMapped]
        int NumPossessions;
        [NotMapped]
        bool StartersRecorded = false;
        [NotMapped]
        int SubsEveryN = 6;
        [NotMapped]
        int Overtimes = 0;
        [NotMapped]
        double SynergyFactor = 0.05;


        //Simulação
        public void GameSim()
        {
            Team[] teams = { HomeTeam, AwayTeam };
            Random random = new Random();
            double randomFactor = random.NextDouble() * (1.1 - 0.9) + 0.9;
            this.NumPossessions = (int)Math.Round((98 + 101) / 2 * randomFactor);

        }


    }
}
