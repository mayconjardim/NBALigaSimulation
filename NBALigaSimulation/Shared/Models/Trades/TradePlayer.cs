using NBALigaSimulation.Shared.Models.Players;

namespace NBALigaSimulation.Shared.Models.Trades
{
    public class TradePlayer
    {

        public int PlayerId { get; set; }
        public Player Player { get; set; }

        public int TradePlayerId { get; set; }
        public Trade Trade { get; set; }

    }
}
