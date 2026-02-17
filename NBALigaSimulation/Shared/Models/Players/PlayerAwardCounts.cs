namespace NBALigaSimulation.Shared.Models.Players
{
    public class PlayerAwardCounts
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public Player Player { get; set; }
        
        public int PlayerOfTheGame { get; set; }
        public int MVP { get; set; }
        public int DPOY { get; set; }
        public int ROY { get; set; } 
        public int SixthManOfTheYear { get; set; } 
        public int MIP { get; set; } 
        public int PlayerOfTheMonth { get; set; }
        public int PlayerOfTheWeek { get; set; }
        public int AllStarGames { get; set; }
        public int TitlesWon { get; set; }
    }
}
