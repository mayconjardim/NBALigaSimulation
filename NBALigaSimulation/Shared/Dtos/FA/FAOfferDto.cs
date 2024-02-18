namespace NBALigaSimulation.Shared.Dtos.FA
{
    public class FAOfferDto
    {

        public int Id { get; set; }
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }

        public int TeamId { get; set; }
        public int Season { get; set; }
        public int Amount { get; set; }
        public int Years { get; set; }
        public bool? Response { get; set; }

    }
}
