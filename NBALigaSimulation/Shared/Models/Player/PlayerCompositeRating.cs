namespace NBALigaSimulation.Shared.Models
{
    public class PlayerCompositeRating
    {

        public Dictionary<string, decimal> Ratings { get; set; }

        public PlayerCompositeRating()
        {
            Ratings = new Dictionary<string, decimal>();
        }

    }
}
