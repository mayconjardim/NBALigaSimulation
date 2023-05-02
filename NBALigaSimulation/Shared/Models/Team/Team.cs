namespace NBALigaSimulation.Shared.Models
{
    public class Team
    {

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string Abrv { get; set; } = string.Empty;
        public List<Player> Players { get; set; } = new List<Player>();                                                                      
    }
}
