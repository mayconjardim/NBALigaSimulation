namespace NBALigaSimulation.Shared.Models.Teams
{
    public class TeamRegularStats
    {

        public int Id { get; set; }
        public int TeamId { get; set; }
        public Team Team { get; set; }
        public int Season { get; set; }
        public int ConfRank { get; set; }
        public int Streak { get; set; }

        public int HomeWins { get; set; }
        public int HomeLosses { get; set; }

        public int RoadWins { get; set; }
        public int RoadLosses { get; set; }

        public int PlayoffWins { get; set; }
        public int PlayoffLosses { get; set; }

        public int Points { get; set; }
        public int AllowedPoints { get; set; }

        public int Steals { get; set; }
        public int AllowedStealS { get; set; }
        
        public int Fouls { get; set; }
        public int AllowedFouls { get; set; }

        public int Rebounds { get; set; }
        public int AllowedRebounds { get; set; }

        public int Assists { get; set; }
        public int AllowedAssists { get; set; }

        public int Blocks { get; set; }
        public int AllowedBlocks { get; set; }

        public int Turnovers { get; set; }
        public int AllowedTurnovers { get; set; }

        public int FGA { get; set; }
        public int FGM { get; set; }
        public int AllowedFGA { get; set; }
        public int AllowedFGM { get; set; }

        public int TPA { get; set; }
        public int TPM { get; set; }
        public int Allowed3PA { get; set; }
        public int Allowed3PM { get; set; }

        public int FTM { get; set; }
        public int FTA { get; set; }
        public int AllowedFTM { get; set; }
        public int AllowedFTA { get; set; }
        

        public double Games
        {
            get
            {
                int games = (HomeWins + HomeLosses + RoadWins + RoadLosses);
                return games;
            }
        }
        
        
        public double WinPct
        {
            get
            {
                double winPct = (double)(HomeWins + RoadWins) / Games;
                return winPct;
            }
        }
    
    }
}
