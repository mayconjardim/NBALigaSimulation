using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Dtos.Teams;

namespace NBALigaSimulation.Shared.Dtos.Trades
{
    public class TradeDto
    {

        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public int TeamOneId { get; set; }
        public string TeamOneName { get; set; }

        public int TeamTwoId { get; set; }
        public string TeamTwoName { get; set; }

        public List<TradePlayerDto> TradePlayers { get; set; }
        public List<PlayerCompleteDto> Players { get; set; }
        public List<TeamDraftPickDto> DraftPicks { get; set; }


        public bool? Response { get; set; } = null;
        public DateTime LastModified { get; set; }

    }
}
