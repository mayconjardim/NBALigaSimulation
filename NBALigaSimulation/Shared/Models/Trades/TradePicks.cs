using NBALigaSimulation.Shared.Models.Teams;

namespace NBALigaSimulation.Shared.Models.Trades
{
    public class TradePicks
    {

        public int DraftPickId { get; set; }
        public TeamDraftPicks DraftPick { get; set; }

        public int TradePickId { get; set; }
        public Trade Trade { get; set; }

    }
}
