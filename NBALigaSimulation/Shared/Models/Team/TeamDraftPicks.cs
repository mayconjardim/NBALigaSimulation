namespace NBALigaSimulation.Shared.Models
{
    public class TeamDraftPicks
    {

        public int Id { get; set; }
        public int TeamId { get; set; }
        public Team Team { get; set; }
        public string Original { get; set; }
        public int Year { get; set; }
        public int Round { get; set; }

    }
}
