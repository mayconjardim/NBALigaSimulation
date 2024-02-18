namespace NBALigaSimulation.Shared.Models.Teams
{
    public class TeamGameplan
    {

        public int Id { get; set; }
        public int TeamId { get; set; }
        public Team Team { get; set; }
        public int Pace { get; set; }
        public int Focus { get; set; }
        public int Motion { get; set; }
        public int Defense { get; set; }

    }
}
