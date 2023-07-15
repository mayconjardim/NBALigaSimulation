namespace NBALigaSimulation.Shared.Models
{
    public class Draft
    {

        public int Id { get; set; }
        public int Season { get; set; }
        public int Round { get; set; }
        public int Pick { get; set; }
        public Team Team { get; set; }
        public string Original { get; set; }
        public int? PlayerId { get; set; }
        public Player? Player { get; set; }
        public DateTime? DateTime { get; set; }

    }

}
