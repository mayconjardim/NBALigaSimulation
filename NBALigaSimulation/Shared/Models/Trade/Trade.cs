namespace NBALigaSimulation.Shared.Models
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


    }
}
