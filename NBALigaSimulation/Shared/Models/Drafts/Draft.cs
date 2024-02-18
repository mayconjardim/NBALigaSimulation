using NBALigaSimulation.Shared.Models.Players;
using NBALigaSimulation.Shared.Models.Teams;

namespace NBALigaSimulation.Shared.Models.Drafts
{
    public class Draft
    {

        public int Id { get; set; }
        public int Season { get; set; }
        public int Round { get; set; }
        public int Pick { get; set; }
        public int TeamId { get; set; }
        public Team Team { get; set; }
        public string Original { get; set; }
        public int? PlayerId { get; set; }
        public Player? Player { get; set; }
        public DateTime? DateTime { get; set; }

    }

}
