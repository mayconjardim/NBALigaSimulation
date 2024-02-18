using NBALigaSimulation.Shared.Models.Teams;

namespace NBALigaSimulation.Shared.Models.Trades
{
    public class Trade
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public int TeamOneId { get; set; }
        public Team TeamOne { get; set; }

        public int TeamTwoId { get; set; }
        public Team TeamTwo { get; set; }

        public List<TradePlayer> TradePlayers { get; set; }
        public List<TradePicks> TradePicks { get; set; }

        public bool? Response { get; set; } = null;
        public DateTime LastModified { get; set; } = DateTime.Now;

    }
}
