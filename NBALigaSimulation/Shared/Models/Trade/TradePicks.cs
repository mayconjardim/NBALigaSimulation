namespace NBALigaSimulation.Shared.Models
{
    public class TradePicks
    {

        public int DraftPickId { get; set; }
        public TeamDraftPicks DraftPick { get; set; }

        public int TradePickId { get; set; }
        public Trade Trade { get; set; }

    }
}
