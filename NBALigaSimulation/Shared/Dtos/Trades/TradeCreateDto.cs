using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Dtos.Teams;

namespace NBALigaSimulation.Shared.Dtos.Trades
{
    public class TradeCreateDto
    {

        public int Id { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public int TeamOneId { get; set; }
        public int TeamTwoId { get; set; }
        public List<TradePlayerDto> TradePlayers { get; set; }
        public List<PlayerCompleteDto> Players { get; set; }
        public List<TeamDraftPickDto> DraftPicks { get; set; }
        public bool? Response { get; set; } = null;
        public DateTime LastModified { get; set; } = DateTime.UtcNow;

    }
}
