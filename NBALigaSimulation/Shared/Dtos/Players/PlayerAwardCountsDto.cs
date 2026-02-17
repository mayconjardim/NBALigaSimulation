namespace NBALigaSimulation.Shared.Dtos.Players
{
    public class PlayerAwardCountsDto
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        
        // Prêmios de jogo
        public int PlayerOfTheGame { get; set; }
        
        // Prêmios de temporada (serão implementados depois)
        public int MVP { get; set; }
        public int DPOY { get; set; } // Defensive Player of the Year
        public int ROY { get; set; } // Rookie of the Year
        public int SixthManOfTheYear { get; set; } // 6MOY
        public int MIP { get; set; } // Most Improved Player
        public int PlayerOfTheMonth { get; set; }
        public int PlayerOfTheWeek { get; set; }
        public int AllStarGames { get; set; }
        public int TitlesWon { get; set; }
    }
}
