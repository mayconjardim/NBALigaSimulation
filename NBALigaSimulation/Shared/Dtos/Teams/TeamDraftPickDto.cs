namespace NBALigaSimulation.Shared.Dtos.Teams
{
    public class TeamDraftPickDto
    {

        public int Id { get; set; }
        public int TeamId { get; set; }
        public string TeamName { get; set; }
        public string Original { get; set; }
        public int Year { get; set; }
        public int Round { get; set; }

    }
}
