using NBALigaSimulation.Shared.Models;

namespace NBALigaSimulation.Shared.Dtos
{
    public class TradeCreateDto
    {

        public int Id { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public int TeamOneId { get; set; }

        public int TeamTwoId { get; set; }

        public List<TradePlayerDto>? TradePlayers { get; set; }

        public bool? Response { get; set; } = null;
        public DateTime LastModified { get; set; } = DateTime.Now;

    }
}
