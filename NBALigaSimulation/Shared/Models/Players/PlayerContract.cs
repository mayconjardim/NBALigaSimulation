namespace NBALigaSimulation.Shared.Models.Players
{
    public class PlayerContract
    {

        public int Id { get; set; }
        public int PlayerId { get; set; }
        public Player Player { get; set; }
        public int Amount { get; set; }
        public int Exp { get; set; }

    }
}
