using NBALigaSimulation.Shared.Models.Players;
using NBALigaSimulation.Shared.Models.Teams;

namespace NBALigaSimulation.Shared.Models.FA
{
    public class FAOffer
    {

        public int Id { get; set; }
        public int PlayerId { get; set; }
        public Player Player { get; set; }
        public int TeamId { get; set; }
        public Team Team { get; set; }
        public int Season { get; set; }
        public int Amount { get; set; }
        public int Years { get; set; }
        public bool? Response { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
           
    }
}
