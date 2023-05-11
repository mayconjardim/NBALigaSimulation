
namespace NBALigaSimulation.Shared.Dtos
{
    public class GameCompleteDto
    {

        public int Id { get; set; }
        public TeamCompleteDto HomeTeam { get; set; }
        public TeamCompleteDto AwayTeam { get; set; }
        public int HomeQ1 { get; set; }
        public int HomeQ2 { get; set; }
        public int HomeQ3 { get; set; }
        public int HomeQ4 { get; set; }
        public int HomeOT { get; set; }
        public int AwayQ1 { get; set; }
        public int AwayQ2 { get; set; }
        public int AwayQ3 { get; set; }
        public int AwayQ4 { get; set; }
        public int AwayOT { get; set; }
        public List<string> PlayByPlay { get; set; } = new List<string>();



    }
}
