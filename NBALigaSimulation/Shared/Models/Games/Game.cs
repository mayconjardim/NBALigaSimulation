using System.ComponentModel.DataAnnotations.Schema;
using NBALigaSimulation.Shared.Engine.Gameplan;
using NBALigaSimulation.Shared.Engine.GameSim.ProbabilityManager;
using NBALigaSimulation.Shared.Engine.Utils;
using NBALigaSimulation.Shared.Models.Players;
using NBALigaSimulation.Shared.Models.Seasons;
using NBALigaSimulation.Shared.Models.Teams;

namespace NBALigaSimulation.Shared.Models.Games
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

        // Atributos Globais da Simulação
        [NotMapped]
        public int NumPossessions; // Quantidade posses de uma partida
        [NotMapped]
        public bool StartersRecorded = false; // Usado para rastrear se os titulares *reais* foram gravados ou não.
        [NotMapped]
        public int SubsEveryN = 6; // Quantas posses esperar antes de fazer substituições
        [NotMapped]
        public int Overtimes = 0; // Números de overtimes 
        [NotMapped]
        public double SynergyFactor = 0.01; // Qual a importância da sinergia?
        [NotMapped]
        public double FatigueFactor = 0.055;
        [NotMapped]
        public int Offense; // Time que está atacando
        [NotMapped]
        public int Defense; // Time que está defendendo
        [NotMapped]
        public double T = 12.00; // Tempo por quarto
        [NotMapped]
        public double Dt = 0; // Tempo decorrido por posse
        [NotMapped]
        public List<List<int>> PtsQrts = new List<List<int>> { new List<int>(), new List<int>() };

      

   

        

 


  
        
        
        
        
    

       

       
        
   


     

       

    }

}