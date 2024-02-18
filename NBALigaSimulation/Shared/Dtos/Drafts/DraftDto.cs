namespace NBALigaSimulation.Shared.Dtos.Drafts
{
    public class DraftDto
    {

        public int Id { get; set; }
        public int Season { get; set; }
        public int Round { get; set; }
        public int Pick { get; set; }
        public string TeamName { get; set; }
        public string TeamAbrv { get; set; }
        public int TeamId { get; set; }
        public string Original { get; set; }
        public string PlayerName { get; set; } = string.Empty;
        public int? PlayerId { get; set; }
        public DateTime? DateTime { get; set; }

    }
}
