namespace NBALigaSimulation.Shared.Models
{
    public class PlayerCompositeRating
    {

        public Dictionary<string, double> Ratings { get; set; }

        public PlayerCompositeRating()
        {
            Ratings = new Dictionary<string, double>();
        }

    }
}
